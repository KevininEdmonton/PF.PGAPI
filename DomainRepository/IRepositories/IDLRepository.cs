using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainRepository.IRepositories
{
    public interface IDLRepository : IDLReadOnlyRepository
    {
        void CreateRange<TEntity>(List<TEntity> entitylist)
            where TEntity : class;//, IEntity;
        void Create<TEntity>(TEntity entity)
            where TEntity : class;//, IEntity;

        void Update<TEntity>(TEntity entity)
            where TEntity : class;//, IEntity;

        void DeleteById<TEntity>(object id, string idname = null)
            where TEntity : class;//, IEntity;

        void Delete<TEntity>(TEntity entity)
            where TEntity : class;//, IEntity;

        int Save();

        Task<int> SaveAsync();

        bool HasChangesToDB();
    }
}
