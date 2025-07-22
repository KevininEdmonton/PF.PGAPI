using K.Common;
using KS.Library.EFDB;
using PFAPI.SupportModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DomainRepository.IRepositories
{
    public interface IPFClientRepository: IDLRepository
    {
        Guid GetClientID();
        Task<bool> SaveChangesAsync(ClaimsPrincipal theUser);
        Task<bool> SaveChangesAsync(string theTrigger);
        Task<PagedData<TReturn>> CreatePagedResults<T, TReturn>(QueryParameter theQueryParameter, IQueryable<T> inputQueryable = null) where T : class where TReturn : class;
        Task<IEnumerable<TReturn>> GetListResult<T, TReturn>(bool includeallchildrendata, IQueryable<T> inputQueryable = null) where T : class where TReturn : class;        
        Task<TReturn> GetOneDataModel<T, TReturn>(QueryParameterMin theQueryParameter, IQueryable<T> inputQueryable = null) where T : class where TReturn : class;

        Task<ZclientUser> GetZClientUserByUserNameAsync(string username, bool includeChildrendata = false);
        //Task<ZclientUser> GetZClientUserByUserNameAsync(string username, bool includeChildrendata = false);
        //List<ClientUserOperations> GetUserOperations(Guid userid, Guid clientid);
        //List<string> GetRootUserPermissionCodes();
        //List<string> GetAdminUserPermissionCodes(Guid clientid);
        //Task<ClientDB.Entities.ZClientAccount> GetZClientAccountAsync(Guid id, bool includeChildrendata = false);
        //Task<ClientDB.Entities.ZClientModule> GetZClientModuleAsync(Guid clientid, Guid id, bool includeChildrendata = false);
        //Task<ClientDB.Entities.ZClientOperation> GetZClientOperationAsync(Guid clientid, Guid id, bool includeChildrendata = false);
    }
}
