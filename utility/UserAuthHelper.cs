using AutoMapper;
using Azure;
using Azure.Core;
using DomainRepository.IRepositories;
using DomainRepository.Repositories;
using PFAPI.SupportModels;
using K.Common;
using KS.Library.EFDB;
using KS.Library.Interface.PFAPI;
using KS.Library.Interface.PFAPI.Domain;
using KS.Library.Interface.PFAPI.Domain.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using PFAPI.SupportModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PFAPI.utility
{
    public class UserAuthHelper : IDisposable
    {
        private IPFClientRepository _repository;
        private IMapper _mapper;
        private ClaimsPrincipal _User;
        private IConfiguration _config;
        private readonly IMemoryCache _memoryCache;

        public UserAuthHelper(IPFClientRepository repository_clientdb, IMapper mapper, ClaimsPrincipal User, IConfiguration config, IMemoryCache memoryCache)
        {
            _repository = repository_clientdb;
            _mapper = mapper;
            _User = User;
            _config = config;
            _memoryCache = memoryCache;
        }
        public void Dispose()
        {
            //    
            _repository = null;
            _mapper = null;
        }

        internal async Task<Tuple<bool, bool, RegisterUserResponseModel, string>> RegisterNewUser(RegisterNewUserModel model)
        {
            if (model == null)
            {
                return new Tuple<bool, bool, RegisterUserResponseModel, string>(false, true, null, "RegisterNewUserModel cannot be null");
            }

            if (model.FirstName.IsNullOrEmptyOrWhiteSpace() || model.UserName.IsNullOrEmptyOrWhiteSpace() || model.LastName.IsNullOrEmptyOrWhiteSpace())
            {
                return new Tuple<bool, bool, RegisterUserResponseModel, string>(false, true, null, "FirstName, LastName and UserName cannot be empty");
            }

            // force username to lowercase  ??? really needed?
            model.UserName = model.UserName.ToLowerInvariant().Trim();

            // 2 veriry zclient user username + client ID
            if (_repository.GetExists<ZclientUser>($"UserName==('{model.UserName}')"))
            {
                return new Tuple<bool, bool, RegisterUserResponseModel, string>(false, true, null, "An account with that email already exists."); // Set return message
            }

            ModelStateDictionary md = new ModelStateDictionary(); // For error messages

            //check password
            if (!validatePasswordRules(model.Password, ref md))
            {
                string errorMessage = "Password does not meet the requirements. ";
                if (md!=null)
                {
                    md.Values.ToList().ForEach(v => errorMessage += v.Errors.FirstOrDefault()?.ErrorMessage + " ");
                }

                return new Tuple<bool, bool, RegisterUserResponseModel, string>(false, true, null, errorMessage);
            }
            RegisterUserResponseModel response = new RegisterUserResponseModel(); // Response model should be returned
         //   using (var transaction = (_repository as PFClientRepository)._context.Database.BeginTransaction())
            {
                try
                {
                    RandomStringGenerator randomStringGen = new RandomStringGenerator();
                    string theSalt = randomStringGen.Generate(32);
                    string passwordHash = EnCryptionHelper.Encrypt(model.Password, theSalt);

                    //var newIdentityUser = new AspNetUser { Id = Guid.NewGuid().ToString(), Email = model.UserName, UserName = model.UserName, PasswordHash = passwordHash, SecurityStamp = theSalt }; // create new AspIdentityUser model

                    //_repository.Create(newIdentityUser);
                    //if (!await _repository.SaveChangesAsync(this.User))
                    //{
                    //    _logger.LogWarning($"Could not save asp net user data to database;");
                    //    return BadRequest();
                    //}

                    model.Password.DefaultIfEmpty();
                    ZclientUser entity = new ZclientUser();
                    entity.FirstName  = model.FirstName; // Set FirstName from model
                    entity.LastName = model.LastName; // Set LastName from model
                    entity.MiddleName = model.MiddleName; // Set MiddleName from model
                    entity.UserName = model.UserName; // Set UserName from model
                    entity.Email = model.UserName; // Set Email from model

                    entity.IdentityId = model.UserName; // Not set by mapper, set Identity ID to ID of new IdentityUser
                    entity.ClientId = Guid.Empty; 

                    entity.IsAccountAdmin = false; // Registering users can not be root
                    entity.IsAccountRoot = false; // Registering users can not be admin
                    entity.IsEnabled = false; // Registering users are not enabled by default // SEAN - isLocked vs isEnabled 
                    entity.IsLocked = true; // Registering users are locked by default                
                    entity.UserType = UserTypes.TemporaryUser.ToString();
                    entity.SecurityClearance = 0; // Registering users have no security clearance by default
                    entity.PasswordHash = passwordHash; // Set PasswordHash to the encrypted password
                    entity.SecurityStamp = theSalt; // Set PasswordSalt to the generated salt


                    _repository.Create(entity); // create ZclientUser in the DB

                    if (!await _repository.SaveChangesAsync(_config["Application:AppName"])) // Save the new user to zClientUsers if it fails return server error 500
                    {
                        return new Tuple<bool, bool, RegisterUserResponseModel, string>(false, false, null, $"Failed to register the new user."); // Failed to create the ZclientUser, return Internal Server Error
                    }

                    response.ID = entity.Id; // Set the response ID to the ID of the new zClientUser
                    response.UserName = entity.UserName; // Set the response UserName to the email address of the newly created zClientUser

                   //     transaction.Commit();
                }
                catch (Exception e)
                {
                    //  transaction.Rollback();      
                    return new Tuple<bool, bool, RegisterUserResponseModel, string>(false, false, null, $"{e.toExceptionString()}");
                }
                               
                return new Tuple<bool, bool, RegisterUserResponseModel, string>(true, false, response, "User registered successfully.");
            }
        }
        internal async Task<Tuple<bool, bool, TokenModel, string>> Login(CredentialModel model, string RemoteIpAddress)
        {
            try
            {
                if (model.UserName.IsNullOrEmptyOrWhiteSpace() || model.Password.IsNullOrEmptyOrWhiteSpace())
                    return new Tuple<bool, bool, TokenModel, string>(false, true, null, "Invalid username or password.");

                //Find Client DB name based on Center DB client account info/using
//                using (IClientRepository _repository_clientdb = new ClientRepository(ClientDBHelper.GetClientDBContext(theClient.Id.ToString(), _config.GetConnectionString("HoBOCustomerConnectionString"), _config["Application:CustomerDBName"]), _config, _mapper, theClient.Id))
                {
                    ZclientUser user = await _repository.GetZClientUserByUserNameAsync(model.UserName);
                    if (user == null)// create
                        return new Tuple<bool, bool, TokenModel, string>(false, true, null, "ZclientUser does NOT exist.");

                    string theSalt = user.SecurityStamp;
                    if (model.Password != EnCryptionHelper.Decrypt(user.PasswordHash, theSalt))
                        return new Tuple<bool, bool, TokenModel, string>(false, true, null, "Invalid username or password.");



                    if (!user.IsEnabled || user.IsLocked)
                        return new Tuple<bool, bool, TokenModel, string>(false, true, null, "User is locked or not enabled.");


                    //if (theClient.Id != theClientUser.ClientId)
                    //{
                    //    return BadRequest("Username does not belong to the domain.");
                    //}

                    return await ProcessTokenandReturn(user, RemoteIpAddress);
                }
            }
            catch (Exception e)
            {
                // _logger.LogError($"Exception thrown while logging in: {e}");
                return new Tuple<bool, bool, TokenModel, string>(false, false, null, $"{e.toExceptionString()}");
            }
        }

        private async Task<Tuple<bool, bool, TokenModel, string>> ProcessTokenandReturn(ZclientUser user, string RemoteIpAddress)
        {
            JwtSecurityToken token = GetJWTToken(user, RemoteIpAddress, out ZclientUserRefreshToken Rtoken, out string msg, out ClientUserwithValidOperations userOperationPermissions);
            if (token == null)
                new Tuple<bool, bool, TokenModel, string>(false, false, null, msg);

            TokenModel result = new TokenModel
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                AccessTokenExpireAt = token.ValidTo,
                RefreshToken = Rtoken.Token,
                ClientAccountID = userOperationPermissions?.ClientAccountID ?? Guid.Empty,
            };

            return new Tuple<bool, bool, TokenModel, string>(true, false, result, "Login successful.");
        }
        private JwtSecurityToken GetJWTToken(ZclientUser user, string RemoteIpAddress, out ZclientUserRefreshToken Rtoken, out string msg, out ClientUserwithValidOperations userOperationPermissions)
        {
            msg = string.Empty;
            userOperationPermissions = null;
            DateTime ProcessTimeUTC = DateTime.UtcNow;
            // Refresh Token                
            Rtoken = HandleRefreshToken(user.Id, RemoteIpAddress, ProcessTimeUTC).Result;
            if (Rtoken == null)
            {
                msg = $"Failed to generate refresh token.";
                return null;
            }

            JwtSecurityToken token = GenerateJwtToken(user, Rtoken.Id, ProcessTimeUTC, out userOperationPermissions);
            if (token == null)
            {
                msg = "Failed to generate token.";
                return null;
            }

            return token;
        }

        private async Task<ZclientUserRefreshToken> HandleRefreshToken(Guid ClientUserID, string RemoteIpAddress, DateTime ProcessTimeUTC, ZclientUserRefreshToken OldToken = null)
        {
           // errmsg = string.Empty;
            try
            {
                if (!int.TryParse(_config["AuthSettings:RefreshTokenExpireDays"], out int expiredays))
                    expiredays = 5;

                ZclientUserRefreshToken Rtoken = new ZclientUserRefreshToken
                {
                    UserId = ClientUserID,
                    Token = WebAPIExtentions.GenerateToken(),
                    RemoteIpAddress = RemoteIpAddress,          //Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Expires = ProcessTimeUTC.AddDays(expiredays)
                };

                _repository.Create(Rtoken);
                if (OldToken != null)
                    _repository.Delete(OldToken);

                if (!await _repository.SaveChangesAsync(_config["Application:AppName"]))
                {
                   // errmsg = "Failed to save refresh token.";
                    return null;
                }

                return Rtoken;
            }
            catch(Exception e)
            {
               // errmsg = $"Exception while handling refresh token: {e.Message}";
                return null;
            }
        }

        private JwtSecurityToken GenerateJwtToken(ZclientUser theClientUser, Guid JTI, DateTime ProcessTimeUTC, out ClientUserwithValidOperations userOperationPermissions)
        {
            try
            {
                userOperationPermissions = new ClientUserwithValidOperations(theClientUser);
                // RepositoryHelper _repoHelper = new RepositoryHelper(_repository, _repository_clientdb, _config, _mapper);

                userOperationPermissions.PermissionCodes = GetUserPermissionCodes(theClientUser);            
                // JWT - Access Token
                //var userClaims = await _userMgr.GetClaimsAsync(user);
                List<Claim> OperationClaims = new List<Claim>();
                if (userOperationPermissions != null && userOperationPermissions.PermissionCodes.HasData())
                {
                    OperationClaims = userOperationPermissions.PermissionCodes.Select(c => new Claim(c, Policy4ModuleOperations.PermissionSelected)).ToList().OrderBy(c => c.Type).ToList();
                }

                var claims = new[]
                {
                      new Claim(JwtRegisteredClaimNames.Sid, theClientUser.Id.ToString()),
                      new Claim(JwtRegisteredClaimNames.Sub, theClientUser.UserName),
                      new Claim(JwtRegisteredClaimNames.Jti, JTI.ToString()),                         
                      //new Claim(JwtRegisteredClaimNames.Email, user.Email) // same as UserName
                      new Claim(SystemStatics.SYS_JWT_User_ClientID, theClientUser.ClientId.ToString()),
                      new Claim(SystemStatics.SYS_JWT_User_IsRoot, theClientUser.IsAccountRoot?"T":"F"),
                      //new Claim(SystemStatics.SYS_JWT_User_IsAdmin, theClientUser.IsAccountAdmin?"T":"F"),

                }.Union(OperationClaims);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["AuthSettings:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                if (!int.TryParse(_config["AuthSettings:AccessTokenExpireMinutes"], out int expireMinute))
                    expireMinute = 15;

                return new JwtSecurityToken(
                  issuer: _config["Application:AppName"],
                  audience: _config["JwtIssuerOptions:Audience"],
                  claims: claims,
                  expires: ProcessTimeUTC.AddMinutes(expireMinute),
                  signingCredentials: creds
                  );
            }
            catch (Exception e)
            {
               // _logger.LogError($"Token generation failed: {e.Message}");
                userOperationPermissions = null;
                return null;
            }
        }

        public List<string> GetUserPermissionCodes(ZclientUser theClientUser)
        {
            List<string> result = new List<string>();
            if (theClientUser.IsAccountRoot)
            {
                result.Add(Policy4ModuleOperations.P_AccountAccessLevel.AccessLevel_Root);
                result.Add(Policy4ModuleOperations.P_AccountAccessLevel.AccessLevel_Admin);
                result.Add(Policy4ModuleOperations.P_AccountAccessLevel.AccessLevel_EveryOne);
            }
            else if (theClientUser.IsAccountAdmin)
            {
                result.Add(Policy4ModuleOperations.P_AccountAccessLevel.AccessLevel_Admin);
                result.Add(Policy4ModuleOperations.P_AccountAccessLevel.AccessLevel_EveryOne);
            }
            else
            {
                result.Add(Policy4ModuleOperations.P_AccountAccessLevel.AccessLevel_EveryOne);
            }
            // temp solution to get all operations
            result.AddRange(Policy4ModuleOperations.GetAllOperationList());
            return result.Distinct().ToList();

            // should be replaced with a better solution - based on the module and operation codes
            ////var zzOperations = _centerContext.ZzOperation.Include(o => o.Module).ToList();
            //List<ZzOperation> zzOperations = _centerRepository.GetQueryable<ZzOperation>().ToList();
            //string thecode;
            //foreach (var aO in zzOperations)
            //{
            //    ZzModule zzModule = _centerRepository.GetById<ZzModule>(aO.ModuleId);
            //    thecode = (zzModule.ModuleCode + Policy4ModuleOperations.ModuleOperationDevider + aO.OperationCode).ToUpper();
            //    result.Add(thecode);
            //}
        }

        private bool validatePasswordRules(string password, ref ModelStateDictionary md)
        {
            bool validNewPassword = true;
            //password meets password rules:
            if (password.Length < 6)
            {
                validNewPassword = false;
                md.AddModelError("Password too short", "Password must be at least 6 characters");
            }

            char[] specialChars = { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '[', ']', '{', '}', ';', ':', '\'', '"', ',', '<', '>', '.', '/', '?', '\\', '|', '~', '`', '*' };
            if (password.IndexOfAny(specialChars) == -1)
            {
                validNewPassword = false;
                md.AddModelError("Password has no special characters", "Password must contain at least one special character");
            }

            if (!(password.Any(char.IsLower)))
            {
                validNewPassword = false;
                md.AddModelError("Password has no lowercase characters", "Password must contain at least one lowercase character");
            }

            if (!(password.Any(char.IsUpper)))
            {
                validNewPassword = false;
                md.AddModelError("Password has no uppercase characters", "Password must contain at least one uppercase character");
            }

            if (!(password.Any(char.IsNumber)))
            {
                validNewPassword = false;
                md.AddModelError("Password has no numbers", "Password must contain at least one number");
            }

            return validNewPassword;
        }


    }
}
