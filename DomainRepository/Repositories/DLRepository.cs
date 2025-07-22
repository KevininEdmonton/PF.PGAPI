using DomainRepository.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainRepository.Repositories
{
    public class DLRepository : DLReadOnlyRepository, IDLRepository
    //where TContext : DbContext
    {
        public DLRepository(DbContext thecontext)
        : base(thecontext)
        {
        }

        public virtual void CreateRange<TEntity>(List<TEntity> entitylist)
            where TEntity : class//, IEntity
        {
            //entity.CreatedDate = DateTime.UtcNow;
            //entity.CreatedBy = createdBy;
            context.Set<TEntity>().AddRange(entitylist);
        }

        public virtual void Create<TEntity>(TEntity entity)
            where TEntity : class//, IEntity
        {
            //entity.CreatedDate = DateTime.UtcNow;
            //entity.CreatedBy = createdBy;
            context.Set<TEntity>().Add(entity);
        }

        public virtual void Update<TEntity>(TEntity entity)
            where TEntity : class//, IEntity
        {
            //entity.ModifiedDate = DateTime.UtcNow;
            //entity.ModifiedBy = modifiedBy;
            context.Set<TEntity>().Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void DeleteById<TEntity>(object id, string idname = null)
            where TEntity : class//, IEntity
        {
            TEntity entity = GetById<TEntity>(id, idname);//context.Set<TEntity>().Find(id);
            Delete(entity);
        }

        public virtual void Delete<TEntity>(TEntity entity)
            where TEntity : class//, IEntity
        {
            var dbSet = context.Set<TEntity>();
            if (context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        public virtual int Save()
        {
            return context.SaveChanges();
        }

        public virtual System.Threading.Tasks.Task<int> SaveAsync()
        {            
            return context.SaveChangesAsync();            
        }

        public virtual bool HasChangesToDB()
        {
            //bool isChanges = false;
            //isChanges = context.ChangeTracker.HasChanges();
            return context.ChangeTracker.HasChanges();
        }
    }
}
