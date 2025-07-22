using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using K.Common;
using DomainRepository.IRepositories;

namespace DomainRepository.Repositories
{
    public class DLReadOnlyRepository : IDLReadOnlyRepository
    //where TContext : DbContext
    {
        protected readonly DbContext context;

        public DLReadOnlyRepository(DbContext thecontext)
        {
            this.context = thecontext;
            //this.context.Configuration.AutoDetectChangesEnabled = false; //disable the change tracking. 

        }
        public virtual IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class
        {
            return context.Set<TEntity>().AsNoTracking();
        }

        public virtual IQueryable<TEntity> GetQueryable<TEntity>(
            string filter = null,
            string orderBy = null,
            //bool ascending = false,
            string includeProperties = null,            
            int? skip = null,
            int? take = null,
            IQueryable<TEntity> query = null)
            where TEntity : class            
        {
            //includeProperties = includeProperties ?? string.Empty;
            if(query==null)
                query = context.Set<TEntity>().AsNoTracking(); // entity returned will not be cached in DBContext

            if (!filter.IsNullOrEmptyOrWhiteSpace())
            {
                //string value = "ZCLIENTMODULE";
                //filter = "ModuleCode =="+"\""+ value+"\"";
                //string value = "f6ffdf96-c724-439e-8d02-1ca36962dea5";
                //filter = "ID ==Guid(\"" + value + "\")";
                query = query.Where(filter.Replace("'", "\""));
                //query = query.Where("ModuleCode==\"ZCLIENTMODULE\"");
            }

            if (!includeProperties.IsNullOrEmptyOrWhiteSpace())
            {
                foreach (var Oneinclude in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {                   
                    query = query.Include(Oneinclude.Trim());                       
                }
            }

            if (!orderBy.IsNullOrEmptyOrWhiteSpace())
            {
                query = query.OrderBy(orderBy);
            }
            else
            {
                if (skip != null && take != null && take > 0)
                    query = query.OrderBy("Id");
            }

            if ((skip.HasValue) && (skip > 0))
            {
                query = query.Skip(skip.Value);
            }

            if ((take.HasValue) && (take > 0))
            {
                query = query.Take(take.Value);
            }

            return query;
        }

        public virtual IEnumerable<TEntity> Get<TEntity>(
            string filter = null,
            string orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            IQueryable<TEntity> query = null)
            where TEntity : class//, IEntity
        {
            return GetQueryable<TEntity>(filter, orderBy, includeProperties, skip, take, query).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync<TEntity>(
            string filter = null,
            string orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            IQueryable<TEntity> query = null)
            where TEntity : class//, IEntity
        {
            return await GetQueryable<TEntity>(filter, orderBy, includeProperties, skip, take, query).ToListAsync();
        }

        public virtual TEntity GetFirst<TEntity>(
           string filter = null,
           string orderBy = null,
           string includeProperties = null,
            IQueryable<TEntity> query = null)
           where TEntity : class//, IEntity
        {
            return GetQueryable<TEntity>(filter, orderBy, includeProperties, null, null, query).FirstOrDefault();
        }

        public virtual async Task<TEntity> GetFirstAsync<TEntity>(
            string filter = null,
            string orderBy = null,
            string includeProperties = null,
            IQueryable<TEntity> query = null)
            where TEntity : class//, IEntity
        {
            return await GetQueryable<TEntity>(filter, orderBy, includeProperties, null, null, query).FirstOrDefaultAsync();
        }

        public virtual TEntity GetById<TEntity>(object id, string idname = null, string includeProperties = null)
            where TEntity : class//, IEntity
        {
            string filter = string.Empty;
            if (!idname.IsNullOrEmptyOrWhiteSpace())
                filter = idname + "=" + id.ToString();

            if (!filter.IsNullOrEmptyOrWhiteSpace() || !includeProperties.IsNullOrEmptyOrWhiteSpace())
            {
                return GetQueryable<TEntity>(filter, null, includeProperties).FirstOrDefault();
            }

            context.Set<TEntity>().AsNoTracking();
            return context.Set<TEntity>().Find(id);
        }

        public virtual async Task<TEntity> GetByIdAsync<TEntity>(object id, string idname = null, string includeProperties = null)
            where TEntity : class//, IEntity
        {
            string filter = string.Empty;
            if (!idname.IsNullOrEmptyOrWhiteSpace())
                filter = idname + "=" + id.ToString();

            if(!filter.IsNullOrEmptyOrWhiteSpace()||!includeProperties.IsNullOrEmptyOrWhiteSpace())
            {
                return await GetQueryable<TEntity>(filter, null, includeProperties).FirstOrDefaultAsync();
            }
            
            context.Set<TEntity>().AsNoTracking();
            return await context.Set<TEntity>().FindAsync(id);
        }

        public virtual int GetCount<TEntity>(string filter = null,
            IQueryable<TEntity> query = null)
            where TEntity : class//, IEntity
        {
            return GetQueryable<TEntity>(filter, null, null, null, null, query).Count();
        }

        public virtual Task<int> GetCountAsync<TEntity>(string filter = null,
            IQueryable<TEntity> query = null)
            where TEntity : class//, IEntity
        {
            return GetQueryable<TEntity>(filter, null, null, null, null, query).CountAsync();
        }

        public virtual bool GetExists<TEntity>(string filter = null,
            IQueryable<TEntity> query = null)
            where TEntity : class//, IEntity
        {
            return GetQueryable<TEntity>(filter, null, null, null, null, query).Any();
        }

        public virtual Task<bool> GetExistsAsync<TEntity>(string filter = null,
            IQueryable<TEntity> query = null)
            where TEntity : class//, IEntity
        {
            return GetQueryable<TEntity>(filter, null, null, null, null, query).AnyAsync();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ReadOnlyRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
