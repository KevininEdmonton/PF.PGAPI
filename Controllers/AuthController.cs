using AutoMapper;
using DomainRepository.IRepositories;
using DomainRepository.Repositories;
using K.Common;
using KS.Library.Interface.PFAPI;
using KS.Library.Interface.PFAPI.Domain.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using PFAPI.utility;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace PFAPI.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiVersion("1.0")]
    [ApiController]   
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private ILogger<AuthController> _logger;
        private IConfiguration _config;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private string errStr;

        public AuthController(ILogger<AuthController> logger                                        
                                        //, IPasswordHasher<AppUser> hasher
                                        , IConfiguration config
                                        , IMapper mapper
                                        )
        {
            _logger = logger;
            _config = config;
            _mapper = mapper;
            if (_jwtSecurityTokenHandler == null)
                _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }



        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="model">Username and Password</param>
        /// <returns>ActionResult of TokenModel</returns>
        /// <remarks>
        /// Sample request:
        ///     POST   
        ///     {
        ///       "userName": "RegisterNewUserTest@_.com",
        ///       "password": "Password1!",
        ///       "firstName": "Firstname",
        ///       "middleName": "Middlename",
        ///       "lastName": "Lastname",
        ///       "domain":"Test"
        ///     }
        /// </remarks>
        /// 
        //No Auth required
        [HttpPost("registernewuser")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)] // OK status if the new user is registered
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // BadRequest if something the user or UI sent prevented the new user from being registered
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Client domain not found (should only happen if consuming the API directly)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Something went wrong but the provided data was good
        public async Task<ActionResult<RegisterUserResponseModel>> RegisterNewUser(RegisterNewUserModel model)
        {
            try
            {
                //ModelStateDictionary md = new ModelStateDictionary(); // For error messages

                using (IPFClientRepository _repository_clientdb = PFClientRepository.CreateRepositoryInstance(_config, _mapper, Guid.Empty))
                using (UserAuthHelper UAManager = new UserAuthHelper(_repository_clientdb, _mapper, null, _config, null))
                {
                    Tuple<bool, bool, RegisterUserResponseModel, string> result = await UAManager.RegisterNewUser(model);
                    if(result==null)
                        return this.StatusCode(StatusCodes.Status500InternalServerError, "Failed to register new user.");
                    if (!result.Item1) // If the user was not registered
                    {
                        if(result.Item2)
                            return BadRequest(result.Item4); // Return BadRequest with the error message
                        else
                            return this.StatusCode(StatusCodes.Status500InternalServerError, result.Item4); // Return NotFound with the error message
                    }
                    
                    return Ok(result.Item3);
                
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception thrown while creating user: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
            }
        }

        /// <summary>
        /// Login API to get access token
        /// </summary>
        /// <param name="model">UserName + Password</param>
        /// <returns>ActionResult of TokenModel</returns>
        /// <remarks>
        /// Sample request:
        ///     POST 
        ///     {
        ///       "userName": "root@test.com",
        ///       "password": "password"
        ///     }
        /// </remarks>
        [HttpPost("login")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TokenModel>> Login(CredentialModel model)
        {
            try
            {
                //ModelStateDictionary md = new ModelStateDictionary(); // For error messages

                using (IPFClientRepository _repository_clientdb = PFClientRepository.CreateRepositoryInstance(_config, _mapper, Guid.Empty))
                using (UserAuthHelper UAManager = new UserAuthHelper(_repository_clientdb, _mapper, null, _config, null))
                {
                    string RemoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
                    Tuple<bool, bool, TokenModel, string> result = await UAManager.Login(model, RemoteIpAddress);
                    if (result == null)
                        return this.StatusCode(StatusCodes.Status500InternalServerError, "Failed to Login.");
                    if (!result.Item1) // If the user was not registered
                    {
                        if (result.Item2)
                            return BadRequest(result.Item4); // Return BadRequest with the error message
                        else
                            return this.StatusCode(StatusCodes.Status500InternalServerError, result.Item4); // Return NotFound with the error message
                    }

                    return Ok(result.Item3);

                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception thrown while creating user: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
            }
        }


        //[HttpPost("forgetpassword")]
        //[Consumes("application/json")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        //[ProducesDefaultResponseType]
        //public async Task<ActionResult<UserPasswordResetRequestModel>> ForgetPassword(ForgetPasswordModel model)
        //{
        //    try
        //    {
        //        ModelStateDictionary md = new ModelStateDictionary(); // Used to pass error message through Helper to UI

        //        #region Get Client Info
        //        ZClientAccountCenter theClient = await GetClientbyDomain(model.Domain);
        //        if (theClient == null)
        //        {
        //            return BadRequest($"Invalid domain {model.Domain}. {errStr}");
        //        }
        //        if (!theClient.IsActive || theClient.IsLocked)
        //            return BadRequest($"Client account is deactived or locked with clientid {model.Domain}");
        //        #endregion

        //        if (model.UserName.IsNullOrEmptyOrWhiteSpace())
        //        {
        //            md.AddModelError("Invalid request", "Invalid username."); // The error message to return 
        //            //return BadRequest(md); // Return the BadRequest with the error message
        //            return this.StatusCode(StatusCodes.Status404NotFound, $"Invalid username.");
        //        }
        //        using (IHoBOClientRepository _repository_clientdb = new HoBOClientRepository(ClientDBHelper.GetClientDBContext(theClient.Id.ToString(), _config.GetConnectionString("HoBOCustomerConnectionString"), _config["Application:CustomerDBName"]), _config, _mapper, theClient.Id))
        //        {
        //            ZclientUser theClientUser = await _repository_clientdb.GetZClientUserByUserNameAsync(model.UserName);
        //            if (theClientUser == null)
        //            {
        //                md.AddModelError("Invalid request", "Username does not exist.");
        //                //return BadRequest(md);
        //                return this.StatusCode(StatusCodes.Status404NotFound, $"Username does not exist.");
        //            }

        //            if (theClientUser.IsLocked || !theClientUser.IsEnabled) // If the user is locked or not enabled send the following error message to the UI
        //            {
        //                md.AddModelError("Invalid request", "Username is locked or not enabled."); // The error message
        //                                                                                           //return BadRequest(md); // Send the BadRequest with the error message
        //                return this.StatusCode(StatusCodes.Status404NotFound, $"Username is locked or not enabled.");
        //            }

        //            if (!int.TryParse(_config["AuthSettings:ResetPasswordTokenExpireHours"], out int ResetPasswordTokenExpireHours))
        //                ResetPasswordTokenExpireHours = 24;

        //            UserPasswordResetRequestModel theModel = new UserPasswordResetRequestModel();
        //            theModel.Email = model.UserName;
        //            theModel.ExpiredAtUTC = DateTime.UtcNow.AddHours(ResetPasswordTokenExpireHours);
        //            theModel.IsValidated = false;
        //            theModel.Token = WebAPIExtentions.GenerateToken();

        //            //do not return the token to the user - Security 
        //            //theModel.Token = WebAPIExtentions.GenerateToken();
        //            UserPasswordResetRequest entity = _mapper.Map<UserPasswordResetRequest>(theModel); //await MappingHelper.MaptoEntity(_repository, _mapper, model);

        //            _repository_clientdb.Create(entity);

        //            if (await _repository_clientdb.SaveChangesAsync(this.User))
        //            {
        //                //get email template - how to get the correct template?
        //                //NotificationTemplate EmailTemplate = new NotificationTemplate();
        //                NotificationTemplate forgotPasswordEmailTemplate = _repository_clientdb.Get<NotificationTemplate>().Where(x => x.TemplateName == "ForgotPassword").FirstOrDefault();

        //                // handle when forgotPasswordEmailTemplate==null;
        //                if(forgotPasswordEmailTemplate == null)
        //                {
        //                    return this.StatusCode(StatusCodes.Status500InternalServerError, "Failed to load email template.");
        //                }

        //                //send email from here/ trigger
        //                using (NotificationAPIHelper theNotificationManager = new NotificationAPIHelper(_config["Application:AppName"], _config["Application:AppKey"], _config["T2BNotificationService:ServiceURL"]))
        //                {
        //                    List<String> username = new List<string>();
        //                    username.Add(model.UserName);
        //                    EmailNotificationRequest Email = new EmailNotificationRequest();
        //                    Email.To = username;
        //                    Email.HtmlBody = forgotPasswordEmailTemplate.EmailHtmlbody;
        //                    Email.Subject = forgotPasswordEmailTemplate.EmailSubject;
        //                    Email.withAttachment = false;
        //                    Email.From = _config["NotificationConfig:SystemContactUsEmail"];


        //                    //get url // need to get domain....
        //                    string url = $"{model.Domain}.{CurrentEnvironDOMAIN}";



        //                    byte[] tokenBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(theModel.Token);
        //                    string encrypted = Convert.ToBase64String(tokenBytes);
        //                    string URLlink = string.Format("{0}://{1}/ResetPassword?email={2}&token={3}", HttpContext.Request.Scheme, url, username.FirstOrDefault(), encrypted); 

        //                    //put link into email
        //                    string toReplace = "{forgotPasswordLink}";

        //                    if (Email.HtmlBody.Contains(toReplace))
        //                    {
        //                        //if (URLlink != null)  // don't need to check
        //                            Email.HtmlBody = Email.HtmlBody.Replace(toReplace, URLlink);
        //                    }

        //                    toReplace = "{Email}";
        //                    if (Email.HtmlBody.Contains(toReplace))
        //                    {
        //                        Email.HtmlBody = Email.HtmlBody.Replace(toReplace, model.UserName);
        //                        //if (username.FirstOrDefault() != null)
        //                        //    Email.HtmlBody = Email.HtmlBody.Replace(toReplace, username.FirstOrDefault());
        //                    }

        //                    Tuple<HttpStatusCode, EmailNotificationResultModel, HttpWebResponse, string> didEmail = theNotificationManager.EmailNotify(Email);

        //                    if (didEmail == null || didEmail.Item2 == null)
        //                    {

        //                        string errmsg = $"Failed to send forgot password email.";
        //                        if(didEmail != null)
        //                        {
        //                            errmsg = $"{errmsg}Status code:{didEmail.Item1};" + (didEmail.Item3 == null ? string.Empty : didEmail.Item3.ToFormatString()) + didEmail.Item4;
        //                        }
        //                        return this.StatusCode(StatusCodes.Status500InternalServerError, errmsg);
        //                    }



        //                }

        //                //Remove Token from return for security - If token is returned access to API allows for unauthenticated reset of any users password to anything that fits the password
        //                entity.Token = string.Empty;

        //                return Ok(_mapper.Map<UserPasswordResetRequestModel>(entity));
        //            }
        //            else
        //            {
        //                _logger.LogWarning($"Could not save data to database.");
        //            }                    

        //            return this.StatusCode(StatusCodes.Status500InternalServerError, null);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"Exception thrown while logging in: {e}");
        //        return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
        //    }
        //}

        //[HttpPost("tokenresetpassword")]
        //[Consumes("application/json")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status422UnprocessableEntity,
        //    Type = typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary))]
        //[ProducesDefaultResponseType]
        //public async Task<ActionResult<ResetPasswordResultModel>> TokenResetPassword(TokenResetPasswordModel model)
        //{
        //    try
        //    {
        //        ModelStateDictionary md = new ModelStateDictionary();
        //        ResetPasswordResultModel result = new ResetPasswordResultModel();
        //        #region Get Client Info
        //        ZClientAccountCenter theClient = await GetClientbyDomain(model.Domain);
        //        if (theClient == null)
        //        {
        //            return BadRequest($"Invalid domain {model.Domain}. {errStr}");
        //        }
        //        if (!theClient.IsActive || theClient.IsLocked)
        //            return BadRequest($"Client account is deactived or locked with clientid {model.Domain}");
        //        #endregion
        //        result.UserName = model.UserName;
        //        using (IHoBOClientRepository _repository_clientdb = new HoBOClientRepository(ClientDBHelper.GetClientDBContext(theClient.Id.ToString(), _config.GetConnectionString("HoBOCustomerConnectionString"), _config["Application:CustomerDBName"]), _config, _mapper, theClient.Id))
        //        {
        //            // 1 verify token
        //            UserPasswordResetRequest theRequest = _repository_clientdb.GetQueryable<UserPasswordResetRequest>().FirstOrDefault(o => o.Token == model.Token);
        //            if (theRequest == null || (!theRequest.Email.EqualsString(model.UserName)) || theRequest.IsValidated)
        //            {
        //                md.AddModelError("Invalid request", "UserName or Token is not valid;");
        //                return BadRequest(md);
        //            }
        //            if (theRequest.ExpiredAtUtc < DateTime.UtcNow)
        //            {
        //                md.AddModelError("Invalid request", "Token is expired;");
        //                return BadRequest(md);
        //            }

        //            // 2 reset password
        //            AspNetUser user = await GetASPNETUserbyUserName(model.UserName, _repository_clientdb);
        //            if (user == null)
        //                return BadRequest($"Invalid username or password. {errStr}");

        //            //does password meet password rules
        //            if (!validatePasswordRules(model.Password, md))
        //            {
        //                return BadRequest(md);
        //            }


        //            //// 2.1 validate the password to be the correct one
        //            string theSalt = user.SecurityStamp;
        //            //if (model.Password != EnCryptionHelper.Decrypt(user.PasswordHash, theSalt))
        //            //    return BadRequest("Invalid username or password.");

        //            // 2.2 reset password
        //            string passwordHash = EnCryptionHelper.Encrypt(model.Password, theSalt);
        //            user.PasswordHash = passwordHash;

        //            _repository_clientdb.Update(user);
        //            if (!await _repository_clientdb.SaveChangesAsync(this.User))
        //            {
        //                _logger.LogWarning($"Could not upadate asp net user data to database;");
        //                return BadRequest();
        //            }
        //            result.IsSuccess = true;

        //            // 2.3 update UserPasswordResetRequest
        //            try
        //            {
        //                _repository_clientdb.Delete(theRequest);
        //                //theRequest.IsValidated = true;
        //                bool success = await _repository_clientdb.SaveChangesAsync(this.User);
        //            }
        //            catch (Exception e)
        //            {
        //                _logger.LogError(e, $"Exception thrown;");
        //                return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
        //            }
        //            return Ok(result);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, $"Exception thrown;");
        //        return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
        //    }
        //}

        //[HttpPost("selfresetpassword")]
        //[Consumes("application/json")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status422UnprocessableEntity,
        //    Type = typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary))]
        //[ProducesDefaultResponseType]
        //public async Task<ActionResult<ResetPasswordResultModel>> selfResetPassword(SelfResetPasswordModel model)
        //{
        //    try
        //    {
        //        ModelStateDictionary md = new ModelStateDictionary();
        //        ResetPasswordResultModel result = new ResetPasswordResultModel();

        //        #region Get Client Info
        //        ZClientAccountCenter theClient = await GetClientbyDomain(model.Domain);
        //        if (theClient == null)
        //        {
        //            return BadRequest($"Invalid domain {model.Domain}. {errStr}");
        //        }
        //        if (!theClient.IsActive || theClient.IsLocked)
        //            return BadRequest($"Client account is deactived or locked with clientid {model.Domain}");
        //        #endregion

        //        result.UserName = model.UserName;


        //        using (IHoBOClientRepository _repository_clientdb = new HoBOClientRepository(ClientDBHelper.GetClientDBContext(theClient.Id.ToString(), _config.GetConnectionString("HoBOCustomerConnectionString"), _config["Application:CustomerDBName"]), _config, _mapper, theClient.Id))
        //        {
        //            // 1 verify username
        //            //var entity = await GetASPNETUserbyUserName(model.UserName, _repository_clientdb);
        //            var entity = _repository_clientdb.GetQueryable<AspNetUser>().FirstOrDefault(u => u.UserName == model.UserName);
        //            if (entity == null)
        //                return BadRequest("Invalid username or password.");

        //            // 2 verify old password
        //            string theSalt = entity.SecurityStamp;
        //            if (model.OldPassword != EnCryptionHelper.Decrypt(entity.PasswordHash, theSalt))
        //                return BadRequest("Invalid username or password.");



        //            if (!validatePasswordRules(model.Password, md))
        //            {
        //                return BadRequest(md);
        //            }

        //            //// 3 velidate the new password
        //            //if (_userMgr.PasswordValidators.HasData())
        //            //{
        //            //    bool hasError = false;
        //            //    foreach (var curPV in _userMgr.PasswordValidators)
        //            //    {
        //            //        var valid = await curPV.ValidateAsync(_userMgr, user, model.Password);
        //            //        if (!valid.Succeeded)
        //            //        {
        //            //            hasError = true;
        //            //            foreach (var e in valid.Errors)
        //            //            {
        //            //                md.AddModelError(e.Code, e.Description);
        //            //            }
        //            //        }
        //            //    }
        //            //    if (hasError)
        //            //        return BadRequest(md);
        //            //}

        //            // 2.2 reset password
        //            entity.PasswordHash = EnCryptionHelper.Encrypt(model.Password, theSalt);

        //            _repository_clientdb.Update(entity);

        //            if (!await _repository_clientdb.SaveChangesAsync(this.User))
        //            {
        //                _logger.LogWarning($"Could not upadate asp net user data to database;");
        //                return BadRequest();
        //            }
        //            result.IsSuccess = true;
        //            return Ok(result);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, $"Exception thrown;");
        //        return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
        //    }
        //}


        //[HttpPost("refreshtoken")]
        //[Consumes("application/json")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        //[ProducesDefaultResponseType]
        ////[ProducesResponseType(StatusCodes.Status404NotFound)]
        ////[ProducesResponseType(StatusCodes.Status422UnprocessableEntity,
        ////    Type = typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary))]
        //public async Task<ActionResult<TokenModel>> Refreshtoken(RefreshTokenModel model)
        //{
        //    try
        //    {
        //        #region Get Client Info
        //        ZClientAccountCenter theClient = await GetClientbyDomain(model.Domain);
        //        if (theClient == null)
        //        {
        //            return BadRequest($"Invalid domain {model.Domain}. {errStr}");
        //        }
        //        if (!theClient.IsActive || theClient.IsLocked)
        //            return BadRequest($"Client account is deactived or locked with clientid {model.Domain}");
        //        #endregion
        //        var cp = ValidateToken(model.AccessToken, GetValidationParameters());
        //        if (cp == null)
        //            return BadRequest("Invalid token.");
        //        Guid JTI = new Guid(cp.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value);

        //        string UserName = cp.Subject;  
        //        if (UserName.IsNullOrEmptyOrWhiteSpace())
        //            return BadRequest("Invalid token; failed to retrieve user info from token.");                

        //        using (IHoBOClientRepository _repository_clientdb = new HoBOClientRepository(ClientDBHelper.GetClientDBContext(theClient.Id.ToString(), _config.GetConnectionString("HoBOCustomerConnectionString"), _config["Application:CustomerDBName"]), _config, _mapper, theClient.Id))
        //        {
        //            AspNetUser user = await GetASPNETUserbyUserName(UserName, _repository_clientdb);
        //            if (user == null)
        //                return BadRequest($"Invalid username or password. {errStr}");

        //            // handle User
        //            ZclientUser theClientUser = await _repository_clientdb.GetFirstAsync<ZclientUser>($"UserName='{user.UserName}'", null, "ZClientUserRefreshToken");
        //            //ZclientUser theClientUser = _repository_clientdb.GetQueryable<ZclientUser>().Where(o => o.UserName == user.UserName).FirstOrDefault();

        //            if (theClientUser == null)// create
        //                return BadRequest("ZclientUser does NOT exist.");

        //            if (!theClientUser.IsEnabled || theClientUser.IsLocked)
        //                return BadRequest("Invalid user.");

        //            ZClientUserRefreshToken existingRefreshToken = theClientUser.ZClientUserRefreshTokens.FirstOrDefault(t => t.Id == JTI && t.Token == model.RefreshToken && t.Expires > DateTime.UtcNow);
        //            if (existingRefreshToken == null)
        //                return BadRequest("Invalid refresh token.");

        //            DateTime ProcessTimeUTC = DateTime.UtcNow;
        //            // Refresh Token
        //            ZClientUserRefreshToken Rtoken = await HandleRefreshToken(theClientUser.Id, ProcessTimeUTC,_repository_clientdb, existingRefreshToken);
        //            if (Rtoken == null)
        //                return BadRequest("Failed to generate refresh token.");

        //            JwtSecurityToken token = GenerateJwtToken(user, theClientUser, Rtoken.Id, ProcessTimeUTC,_repository_clientdb, out ClientUserwithValidOperations userOperationPermissions);
        //            if (token == null)
        //                return BadRequest("Failed to generate token.");

        //            return Ok(new TokenModel
        //            {
        //                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
        //                AccessTokenExpireAt = token.ValidTo,
        //                RefreshToken = Rtoken.Token,
        //                ClientAccountID = theClientUser.ClientId
        //            });
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"Exception thrown while logging in: {e}");
        //        return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
        //    }
        //}




        //#region supporting Login
        //// need to retrieve ModuleOperationsInfo 
        //private async Task<JwtSecurityToken> GenerateJwtToken(AppUser user, ZclientUser theClientUser, Guid JTI, DateTime ProcessTimeUTC, ClientUserwithValidOperations userOperationPermissions)
        //{
        //    try
        //    {
        //        // JWT - Access Token
        //        var userClaims = await _userMgr.GetClaimsAsync(user);
        //        List<Claim> OperationClaims = new List<Claim>();
        //        if (userOperationPermissions != null && userOperationPermissions.PermissionCodes.HasData())
        //        {
        //            OperationClaims = userOperationPermissions.PermissionCodes.Select(c => new Claim(c, Policy4ModuleOperations.PermissionSelected)).ToList();
        //        }

        //        var claims = new[]
        //        {
        //              new Claim(JwtRegisteredClaimNames.Sid, theClientUser.Id.ToString()),
        //              new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        //              new Claim(JwtRegisteredClaimNames.Jti, JTI.ToString()),                         
        //              //new Claim(JwtRegisteredClaimNames.Email, user.Email) // same as UserName
        //              new Claim(SystemStatics.SYS_JWT_User_ClientID, theClientUser.ClientId.ToString()),
        //              new Claim(SystemStatics.SYS_JWT_User_IsRoot, theClientUser.IsAccountRoot?"T":"F"),
        //              //new Claim(SystemStatics.SYS_JWT_User_IsAdmin, theClientUser.IsAccountAdmin?"T":"F"),

        //        }.Union(userClaims).Union(OperationClaims);

        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["AuthSettings:SecretKey"]));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //        if (!int.TryParse(_config["AuthSettings:AccessTokenExpireMinutes"], out int expireMinute))
        //            expireMinute = 15;

        //        return new JwtSecurityToken(
        //          issuer: _config["Application:AppName"],
        //          audience: _config["JwtIssuerOptions:Audience"],
        //          claims: claims,
        //          expires: ProcessTimeUTC.AddMinutes(expireMinute),
        //          signingCredentials: creds
        //          );
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}
        //private JwtSecurityToken GenerateJwtToken(AspNetUser user, ZclientUser theClientUser, Guid JTI, DateTime ProcessTimeUTC, IHoBOClientRepository _repository_clientdb, out ClientUserwithValidOperations userOperationPermissions)
        //{
        //    try
        //    {
        //        userOperationPermissions = new ClientUserwithValidOperations(theClientUser);
        //        RepositoryHelper _repoHelper = new RepositoryHelper(_repository, _repository_clientdb, _config, _mapper);

        //        if (userOperationPermissions.IsAccountRoot)
        //        {
        //            //userOperationPermissions.PermissionCodes = _repository.GetRootUserPermissionCodes();
        //            userOperationPermissions.PermissionCodes = _repoHelper.GetRootUserPermissionCodes();
        //            userOperationPermissions.ModuleOperationsInfo = _repoHelper.GetRootUserModuleOperationsInfo();
        //        }
        //        else if (theClientUser.IsAccountAdmin)
        //        {
        //            userOperationPermissions.PermissionCodes = _repoHelper.GetAdminUserPermissionCodes(theClientUser.ClientId);
        //            userOperationPermissions.ModuleOperationsInfo = _repoHelper.GetAdminUserModuleOperationsInfo(theClientUser.ClientId);
        //        }
        //        else
        //        {
        //            userOperationPermissions.ValidOperations = _repoHelper.GetUserOperations(theClientUser.Id, theClientUser.ClientId);
        //            List<string> Pcodes = new List<string>();
        //            Pcodes.Add(Policy4ModuleOperations.P_AccountAccessLevel.AccessLevel_EveryOne);
        //            Pcodes.AddRange(userOperationPermissions.ValidOperations.Where(o => o.IsValidOperation).Select(o => o.OperationPermissionCode).Distinct().ToList());
        //            userOperationPermissions.PermissionCodes = Pcodes;
        //            userOperationPermissions.ModuleOperationsInfo = _repoHelper.GetUserModuleOperationsInfo1(theClientUser.Id, theClientUser.ClientId);
        //        }

        //        // JWT - Access Token
        //        //var userClaims = await _userMgr.GetClaimsAsync(user);
        //        List<Claim> OperationClaims = new List<Claim>();
        //        if (userOperationPermissions != null && userOperationPermissions.PermissionCodes.HasData())
        //        {
        //            OperationClaims = userOperationPermissions.PermissionCodes.Select(c => new Claim(c, Policy4ModuleOperations.PermissionSelected)).ToList().OrderBy(c=>c.Type).ToList();
        //        }

        //        var claims = new[]
        //        {
        //              new Claim(JwtRegisteredClaimNames.Sid, theClientUser.Id.ToString()),
        //              new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        //              new Claim(JwtRegisteredClaimNames.Jti, JTI.ToString()),                         
        //              //new Claim(JwtRegisteredClaimNames.Email, user.Email) // same as UserName
        //              new Claim(SystemStatics.SYS_JWT_User_ClientID, theClientUser.ClientId.ToString()),
        //              new Claim(SystemStatics.SYS_JWT_User_IsRoot, theClientUser.IsAccountRoot?"T":"F"),
        //              //new Claim(SystemStatics.SYS_JWT_User_IsAdmin, theClientUser.IsAccountAdmin?"T":"F"),

        //        }.Union(OperationClaims);

        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["AuthSettings:SecretKey"]));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //        if (!int.TryParse(_config["AuthSettings:AccessTokenExpireMinutes"], out int expireMinute))
        //            expireMinute = 15;

        //        return new JwtSecurityToken(
        //          issuer: _config["Application:AppName"],
        //          audience: _config["JwtIssuerOptions:Audience"],
        //          claims: claims,
        //          expires: ProcessTimeUTC.AddMinutes(expireMinute),
        //          signingCredentials: creds
        //          );
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"Token generation failed: {e.Message}");
        //        userOperationPermissions = null;
        //        return null;
        //    }
        //}
        //private async Task<ZClientUserRefreshToken> HandleRefreshToken(Guid ClientUserID, DateTime ProcessTimeUTC, IHoBOClientRepository _repository_clientdb, ZClientUserRefreshToken OldToken = null)
        //{
        //    try
        //    {
        //        if (!int.TryParse(_config["AuthSettings:RefreshTokenExpireDays"], out int expiredays))
        //            expiredays = 5;

        //        ZClientUserRefreshToken Rtoken = new ZClientUserRefreshToken
        //        {
        //            UserId = ClientUserID,
        //            Token = WebAPIExtentions.GenerateToken(),
        //            RemoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
        //            Expires = ProcessTimeUTC.AddDays(expiredays)
        //        };

        //        _repository_clientdb.Create(Rtoken);
        //        if (OldToken != null)
        //            _repository_clientdb.Delete(OldToken);

        //        if (!await _repository_clientdb.SaveChangesAsync(this.User))
        //            return null;
        //        return Rtoken;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        //private JwtSecurityToken ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
        //{
        //    try
        //    {
        //        var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        //        if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //            throw new SecurityTokenException("Invalid token");

        //        return securityToken as JwtSecurityToken;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"Token validation failed: {e.Message}");
        //        return null;
        //    }
        //}
        //private TokenValidationParameters GetValidationParameters()
        //{
        //    return new TokenValidationParameters()
        //    {
        //        ValidateLifetime = false, // we check expired tokens here
        //        ValidateAudience = true,
        //        ValidateIssuer = true,
        //        ValidateIssuerSigningKey = true,
        //        ValidIssuer = _config["Application:AppName"],
        //        ValidAudience = _config["JwtIssuerOptions:Audience"],
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["AuthSettings:SecretKey"]))
        //    };
        //}
        //private async Task<ActionResult<TokenModel>> ProcessTokenandReturn(AspNetUser user, ZclientUser theClientUser,IHoBOClientRepository _repository_clientdb)
        //{
        //    JwtSecurityToken token = GetJWTToken(user, theClientUser, _repository_clientdb, out ZClientUserRefreshToken Rtoken, out string msg, out ClientUserwithValidOperations userOperationPermissions);
        //    if (token == null)
        //        return BadRequest(msg);

        //    return Ok(new TokenModel
        //    {
        //        AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
        //        AccessTokenExpireAt = token.ValidTo,
        //        RefreshToken = Rtoken.Token,
        //        ClientAccountID = theClientUser.ClientId
        //    });
        //}
        //private JwtSecurityToken GetJWTToken(AspNetUser user, ZclientUser theClientUser, IHoBOClientRepository _repository_clientdb, out ZClientUserRefreshToken Rtoken, out string msg, out ClientUserwithValidOperations userOperationPermissions)
        //{
        //    msg = string.Empty;
        //    userOperationPermissions = null;
        //    DateTime ProcessTimeUTC = DateTime.UtcNow;
        //    // Refresh Token                
        //    Rtoken = HandleRefreshToken(theClientUser.Id, ProcessTimeUTC, _repository_clientdb).Result;
        //    if (Rtoken == null)
        //    {
        //        msg = "Failed to generate refresh token.";
        //        return null;
        //    }

        //    JwtSecurityToken token = GenerateJwtToken(user, theClientUser, Rtoken.Id, ProcessTimeUTC, _repository_clientdb, out userOperationPermissions);
        //    if (token == null)
        //    {
        //        msg = "Failed to generate token.";
        //        return null;
        //    }

        //    return token;
        //}
        //private async Task<ActionResult<TokenWithSupportinginfoModel>> ProcessTokenandReturnUIInfo(AspNetUser user, ZclientUser theClientUser, IHoBOClientRepository _repository_clientdb)
        //{
        //    JwtSecurityToken token = GetJWTToken(user, theClientUser, _repository_clientdb, out ZClientUserRefreshToken Rtoken, out string msg, out ClientUserwithValidOperations userOperationPermissions);
        //    if (token == null)
        //        return BadRequest(msg);

        //    UISupportinginfoModel theUISUpportingInfo = new UISupportinginfoModel();
        //    try
        //    {
        //        //ClientInfo
        //        ZClientAccount ClientAccount = await _repository_clientdb.GetZClientAccountAsync(theClientUser.ClientId);
        //        if (ClientAccount == null)
        //            return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to retrieve ZClientAccount[{theClientUser.ClientId}]");

        //        theUISUpportingInfo.ClientInfo = MaptoUIInfo(ClientAccount);
        //        ZCompanyDefaultValue Zcdv = _repository_clientdb.GetQueryable<ZCompanyDefaultValue>().Where(s=>s.Name== "IsAllowsHybridApprovals").FirstOrDefault();
        //        if (Zcdv != null)
        //        {
        //            theUISUpportingInfo.ClientInfo.IsAllowHybridApprovals = Convert.ToBoolean(Zcdv.Value);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"Exception thrown in ProcessTokenandReturnUIInfo: {e}");
        //    }

        //    ZClientUserCompany ZCUC = null;
        //    try
        //    {
        //        ZCUC = _repository_clientdb.GetQueryable<ZClientUserCompany>().FirstOrDefault(c => c.UserId == theClientUser.Id && c.IsDefaultCompany);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"Exception thrown in ProcessTokenandReturnUIInfo: {e}");
        //    }

        //    try
        //    {
        //        //UserInfo
        //        theUISUpportingInfo.UserInfo = MaptoUIInfo(theClientUser);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"Exception thrown in ProcessTokenandReturnUIInfo: {e}");
        //    }

        //    try
        //    {
        //        //ModuleOperationsInfo
        //        theUISUpportingInfo.ModuleOperationsInfo = userOperationPermissions.ModuleOperationsInfo;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"Exception thrown in ProcessTokenandReturnUIInfo: {e}");
        //    }

        //    return Ok(new TokenWithSupportinginfoModel
        //    {
        //        AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
        //        AccessTokenExpireAt = token.ValidTo,
        //        RefreshToken = Rtoken.Token,
        //        ClientAccountID = theClientUser.ClientId,
        //        DefaultCompanyID = (ZCUC == null) ? Guid.Empty : (ZCUC.CompanyId),
        //        UISUpportingInfo = theUISUpportingInfo
        //    });
        //}

        //#endregion

        //#region supporting VerifyAppkey 
        //private bool VerifyAppKey(ZClientAppkey theAppKey, string theAppKeyString)
        //{
        //    if (theAppKey == null || theAppKeyString.IsNullOrEmptyOrWhiteSpace())
        //        return false;
        //    string theSalt = _config["AuthSettings:SecretKey"] + theAppKey.Id.ToString();
        //    return EnCryptionHelper.Encrypt(theAppKeyString, theSalt) == theAppKey.HashedAppkey;
        //}
        //#endregion
        //private async Task<ZClientAccountCenter> GetClientbyDomain(string clientDomain)
        //{
        //    return LogicHelper.GetClientbyDomain(_repository, clientDomain, out errStr);
        //    //errStr = "";
        //    //if (clientDomain.IsNullOrEmptyOrWhiteSpace())
        //    //{
        //    //    errStr = "Invalid data: clientDomain must be provided.";
        //    //    return null;
        //    //}

        //    //clientDomain = WebUtility.UrlDecode(clientDomain.Trim().ToLower());

        //    //if (clientDomain.ToLower().StartsWith("https://"))
        //    //{
        //    //    clientDomain = clientDomain.Substring(8);
        //    //}
        //    //else if (clientDomain.ToLower().StartsWith("http://"))
        //    //{
        //    //    clientDomain = clientDomain.Substring(7);
        //    //}
        //    //else if (clientDomain.ToLower().StartsWith("."))
        //    //{
        //    //    clientDomain = clientDomain.Substring(1);
        //    //}

        //    //if (clientDomain.IsNullOrEmptyOrWhiteSpace())
        //    //{
        //    //    errStr = "Invalid data: clientDomain must be provided.";
        //    //    return null;
        //    //}

        //    //IQueryable<ZClientAccountCenter> theQuery = _repository.GetQueryable<ZClientAccountCenter>();
        //    //if (theQuery == null)
        //    //{
        //    //    errStr = "Something is wrong with data repository.";
        //    //    return null;
        //    //}
        //    //// check the whole domain
        //    //var newQuery = theQuery.Where(o => o.Domain == clientDomain);
        //    //QueryParameterMin theQueryParameter = new QueryParameterMin(HoBOStatic.SYS_Default_QP_IncludeAllChildrenData, HoBOStatic.SYS_Default_QP_IncludeData);
        //    //ZClientAccountCenter result = await _repository.GetOneDataModel<ZClientAccountCenter, ZClientAccountCenter>(theQueryParameter, newQuery);

        //    //if (result != null)
        //    //    return result;

        //    //int loc = clientDomain.IndexOf('.');
        //    //if (loc <= 0)
        //    //{
        //    //    return null;
        //    //}
        //    //// get first part of the domain
        //    //clientDomain = clientDomain.Substring(0, loc);
        //    //newQuery = theQuery.Where(o => o.Domain == clientDomain);
        //    //return await _repository.GetOneDataModel<ZClientAccountCenter, ZClientAccountCenter>(theQueryParameter, newQuery);            
        //}
        //private async Task<ClientDB.Entities.AspNetUser> GetASPNETUserbyUserName(string userName, IHoBOClientRepository _repository_clientdb)
        //{
        //    errStr = "";
        //    if (userName.IsNullOrEmptyOrWhiteSpace())
        //    {
        //        errStr = "Invalid data: UserName must be provided.";
        //        return null;
        //    }

        //    IQueryable<AspNetUser> theQuery = _repository_clientdb.GetQueryable<AspNetUser>();
        //    if (theQuery == null)
        //    {
        //        errStr = "Something is wrong with data repository.";
        //        return null;
        //    }
        //    // check the whole domain
        //    var newQuery = theQuery.Where(o => o.UserName == userName);
        //    QueryParameterMin theQueryParameter = new QueryParameterMin(HoBOStatic.SYS_Default_QP_IncludeAllChildrenData, HoBOStatic.SYS_Default_QP_IncludeData);
        //    AspNetUser result = await _repository_clientdb.GetOneDataModel<AspNetUser, AspNetUser>(theQueryParameter, newQuery);

        //    if (result != null)
        //        return result;
        //    return result;
        //}




        //private bool validatePasswordRules(string password, ModelStateDictionary md)
        //{
        //    bool validNewPassword = true;
        //    //password meets password rules:
        //    if (password.Length < 6)
        //    {
        //        validNewPassword = false;
        //        md.AddModelError("Password too short", "Password must be at least 6 characters");
        //    }

        //    char[] specialChars = { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '[', ']', '{', '}', ';', ':', '\'', '"', ',', '<', '>', '.', '/', '?', '\\', '|', '~', '`', '*' };
        //    if (password.IndexOfAny(specialChars) == -1)
        //    {
        //        validNewPassword = false;
        //        md.AddModelError("Password has no special characters", "Password must contain at least one special character");
        //    }

        //    if (!(password.Any(char.IsLower)))
        //    {
        //        validNewPassword = false;
        //        md.AddModelError("Password has no lowercase characters", "Password must contain at least one lowercase character");
        //    }

        //    if (!(password.Any(char.IsUpper)))
        //    {
        //        validNewPassword = false;
        //        md.AddModelError("Password has no uppercase characters", "Password must contain at least one uppercase character");
        //    }

        //    if (!(password.Any(char.IsNumber)))
        //    {
        //        validNewPassword = false;
        //        md.AddModelError("Password has no numbers", "Password must contain at least one number");
        //    }

        //    return validNewPassword;            
        //}

    }
}