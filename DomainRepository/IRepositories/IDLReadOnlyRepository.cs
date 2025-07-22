using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainRepository.IRepositories
{
    public interface IDLReadOnlyRepository : IDisposable
    {
        //IEnumerable<TEntity> GetAll<TEntity>(
        //    string orderBy = null,
        //    bool ascending = false,
        //    string includeProperties = null,
        //    string includedata = null,
        //    int? skip = null,
        //    int? take = null)
        //    where TEntity : class;//, IEntity;

        //Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
        //    string orderBy = null,
        //    bool ascending = false,
        //    string includeProperties = null,
        //    string includedata = null,
        //    int? skip = null,
        //    int? take = null)
        //    where TEntity : class;//, IEntity;

        IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class;
        IQueryable<TEntity> GetQueryable<TEntity>(
            string filter = null,
            string orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            IQueryable<TEntity> query = null)
            where TEntity : class;

        IEnumerable<TEntity> Get<TEntity>(
            string filter = null,
            string orderBy = null,
            //bool ascending = false,
            string includeProperties = null,            
            int? skip = null,
            int? take = null,
            IQueryable<TEntity> query = null)
            where TEntity : class;//, IEntity;

        Task<IEnumerable<TEntity>> GetAsync<TEntity>(
            string filter = null,
            string orderBy = null,
            //bool ascending = false,
            string includeProperties = null,            
            int? skip = null,
            int? take = null,
            IQueryable<TEntity> query = null)
            where TEntity : class;//, IEntity;

        //TEntity GetOne<TEntity>(
        //    string filter = null,
        //    string includeProperties = null,
        //    string includedata = null)
        //    where TEntity : class;//, IEntity;

        //Task<TEntity> GetOneAsync<TEntity>(
        //    string filter = null,
        //    string includeProperties = null,
        //    string includedata = null)
        //    where TEntity : class;//, IEntity;

        TEntity GetFirst<TEntity>(
            string filter = null,
            string orderBy = null,
            //bool ascending = false,
            string includeProperties = null,
            IQueryable<TEntity> query = null)
            where TEntity : class;//, IEntity;

        Task<TEntity> GetFirstAsync<TEntity>(
            string filter = null,
            string orderBy = null,
           // bool ascending = false,
            string includeProperties = null,
            IQueryable<TEntity> query = null)
            where TEntity : class;//, IEntity;

        TEntity GetById<TEntity>(object id, string idname= null, string includeProperties = null)
            where TEntity : class;//, IEntity;

        Task<TEntity> GetByIdAsync<TEntity>(object id, string idname = null, string includeProperties = null)
            where TEntity : class;//, IEntity;

        int GetCount<TEntity>(string filter = null,
            IQueryable<TEntity> query = null)
            where TEntity : class;//, IEntity;

        Task<int> GetCountAsync<TEntity>(string filter = null,
            IQueryable<TEntity> query = null)
            where TEntity : class;//, IEntity;

        bool GetExists<TEntity>(string filter = null,
            IQueryable<TEntity> query = null)
            where TEntity : class;//, IEntity;

        Task<bool> GetExistsAsync<TEntity>(string filter = null,
            IQueryable<TEntity> query = null)
            where TEntity : class;//, IEntity;
    }
}
