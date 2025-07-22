using PFAPI.SupportModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainRepository.IRepositories
{
    public interface IUserRepository : IRepository<ZClientUser>
    {
        Task<ZClientUser> Create(string firstName, string lastName, string email, string userName, string password);
        Task<ZClientUser> FindByName(string userName);
        Task<bool> CheckPassword(ZClientUser user, string password);
    }
}
