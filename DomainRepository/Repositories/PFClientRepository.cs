using AutoMapper;
using DomainRepository.IRepositories;
using K.Common;
using KS.Library.EFDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using PFAPI.SupportModels;
using PFAPI.utility;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DomainRepository.Repositories
{
    public class PFClientRepository : DLRepository, IPFClientRepository
    {
        public readonly PfsandBoxContext _context;
        private IConfiguration _config;
        private readonly IMapper _mapper;
        public Guid _clientId { get; set; }

        public PFClientRepository(PfsandBoxContext context, IConfiguration config, IMapper mapper, Guid clientId)
            : base(context)
        {
            _context = context;
            _config = config;
            _mapper = mapper;
            _clientId = clientId;
        }

        public static PFClientRepository CreateRepositoryInstance(ClaimsPrincipal User, IConfiguration _config, IMapper _mapper)
        {
            Guid theClientID = new Guid(User.GetClaimValue(SystemStatics.SYS_JWT_User_ClientID));
            return new PFClientRepository(User.GetClientDBContext(_config.GetConnectionString("PFConnectionString"), _config["Application:CustomerDBName"]), _config, _mapper, theClientID);
        }

        public static PFClientRepository CreateRepositoryInstance(IConfiguration _config, IMapper _mapper, Guid theClientID)
        {
            string theClientIDStr = _config.GetConnectionString("PFConnectionString");
            if (theClientIDStr.IsNullOrEmptyOrWhiteSpace())
                throw new ArgumentException("The connection string 'PFConnectionString' is not set in the configuration.");
            return new PFClientRepository(theClientIDStr.GetClientDBContext(), _config, _mapper, theClientID);
        }
        public Guid GetClientID()
        {
            return _clientId;
        }
        public async Task<bool> SaveChangesAsync(ClaimsPrincipal theUser)
        {
            WebAPIExtentions.AddAuitInfo(_context, theUser.GetClaimValue(JwtRegisteredClaimNames.Sid), _config["Application:AppName"]);
            return await SaveAsync() > 0;
            //(await _context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> SaveChangesAsync(string theTrigger)
        {
            WebAPIExtentions.AddAuitInfo(_context, theTrigger, _config["Application:AppName"]);
            return await SaveAsync() > 0;
            //(await _context.SaveChangesAsync()) > 0;
        }

        public async Task<PagedData<TReturn>> CreatePagedResults<T, TReturn>(QueryParameter theQueryParameter, IQueryable<T> inputQueryable = null)
            where T : class//, IEntity
            where TReturn : class
        {
            IQueryable<T> dataqueryable;
            if (theQueryParameter.includeallchildrendata)
            {
                dataqueryable = GetQueryable<T>(theQueryParameter.filter, theQueryParameter.orderby, null, theQueryParameter.Skip, theQueryParameter.Take, inputQueryable);
                dataqueryable = AddChildDataIncludeQuery(dataqueryable);
            }
            else
            {
                dataqueryable = GetQueryable<T>(theQueryParameter.filter, theQueryParameter.orderby, theQueryParameter.includedata, theQueryParameter.Skip, theQueryParameter.Take, inputQueryable);
                //dataqueryable = AddUDFIncludeQuery(dataqueryable);
            }

            try
            {
                var data = await dataqueryable.ToArrayAsync();


                int totalNumberOfRecords = await GetCountAsync<T>(theQueryParameter.filter, inputQueryable);
                var mod = totalNumberOfRecords % theQueryParameter.pagesize;
                var totalPageCount = (totalNumberOfRecords / theQueryParameter.pagesize) + (mod == 0 ? 0 : 1);

                #region handle Page URLs
                string basepage = null, curpage = null;
                if (!theQueryParameter.theURL.IsNullOrEmptyOrWhiteSpace())
                {
                    QueryParameter theQP = theQueryParameter.Clone();
                    string subURL = theQP.ToString();
                    if (!subURL.IsNullOrEmptyOrWhiteSpace())
                        curpage = theQP.theURL + "?" + subURL;
                    else
                        curpage = theQP.theURL;

                    theQP = theQueryParameter.Clone();
                    theQP.page = SystemStatics.SYS_Default_QP_Page;
                    subURL = theQP.ToString();
                    if (!subURL.IsNullOrEmptyOrWhiteSpace())
                        basepage = theQP.theURL + "?" + subURL;
                    else
                        basepage = theQP.theURL;
                }
                #endregion
                return new PagedData<TReturn>
                {
                    PageNum = theQueryParameter.page,
                    PageSize = theQueryParameter.pagesize,
                    PagesCount = totalPageCount,
                    RecordsCount = totalNumberOfRecords,
                    OrderBy = theQueryParameter.orderby,

                    //PrevPageUrl = prevpage,
                    PageUrl = curpage,
                    BaseUrl = basepage,

                    Data = data.HasData() ? _mapper.Map<TReturn[]>(data) : null
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<IEnumerable<TReturn>> GetListResult<T, TReturn>(bool includeallchildrendata, IQueryable<T> inputQueryable = null)
                    where T : class//, IEntity
                    where TReturn : class
        {
            IQueryable<T> dataqueryable = inputQueryable;
            if (inputQueryable == null)
                dataqueryable = GetQueryable<T>();

            if (includeallchildrendata)
            {
                dataqueryable = AddChildDataIncludeQuery(dataqueryable);
            }

            var data = await dataqueryable.ToArrayAsync();
            return data.HasData() ? _mapper.Map<TReturn[]>(data) : null;
        }

        public async Task<TReturn> GetOneDataModel<T, TReturn>(QueryParameterMin theQueryParameter, IQueryable<T> inputQueryable = null) where T : class where TReturn : class
        {
            IQueryable<T> dataqueryable;
            if (theQueryParameter.includeallchildrendata)
            {
                dataqueryable = GetQueryable<T>(null, null, null, null, null, inputQueryable);
                dataqueryable = AddChildDataIncludeQuery(dataqueryable);
            }
            else
            {
                dataqueryable = GetQueryable<T>(null, null, theQueryParameter.includedata, null, null, inputQueryable);
            }
            var data = await dataqueryable.FirstOrDefaultAsync();

            return _mapper.Map<TReturn>(data);
        }
        private IQueryable<T> AddChildDataIncludeQuery<T>(IQueryable<T> queryable) where T : class
        {
            // rules: if children could have a lot data(like more than 100), then it should NOT be supported by include (could cause performance issue)
            if(queryable == null)
                return null;

            var curType = typeof(T);
            switch (curType.Name)
            {
                // Example
                //case "?????Module":
                //    return (queryable as IQueryable<ZClientModule>)
                //                .Include(c => c.ZClientOperation)
                //                    .ThenInclude(c => c.ZClientRoleOperation)
                //                 as IQueryable<T>;

                case "Ktopic":
                    return (queryable as IQueryable<Ktopic>)
                                .Include(c => c.KtopicComments)
                                 as IQueryable<T>;

            }
            return queryable;
        }

        public bool HasChanges(ClaimsPrincipal theUser)
        {
            WebAPIExtentions.AddAuitInfo(_context, theUser.GetClaimValue(JwtRegisteredClaimNames.Sid), _config["Application:AppName"]);
            return HasChangesToDB();
        }

    }
}
