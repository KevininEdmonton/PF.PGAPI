using KS.Library.EFDB;
using PFAPI.SupportModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainRepository.IRepositories
{
    public interface IUserRepository : IRepository<ZclientUser>
    {
        Task<ZclientUser> Create(string firstName, string lastName, string email, string userName, string password);
        Task<ZclientUser> FindByName(string userName);
        Task<bool> CheckPassword(ZclientUser user, string password);
    }
}
