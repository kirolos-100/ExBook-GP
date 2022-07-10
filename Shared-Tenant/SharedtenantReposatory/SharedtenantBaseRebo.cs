﻿
using Microsoft.EntityFrameworkCore;
using SharedTenant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SharedTenant.SharedtenantReposatory
{
    public class SharedtenantBaseRebo<TEntity> where TEntity : class
    {
        private readonly SharedtenantContext DbContext;
        protected DbSet<TEntity> DbSet;
        public SharedtenantBaseRebo(SharedtenantContext context)
        {
            DbContext = context;
            DbSet = DbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll()
        {
            IQueryable<TEntity> entities = DbSet;
            return entities;
        }
        public virtual TEntity GetOne(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetMany(where, includeProperties).FirstOrDefault();
        }
        public virtual IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> where = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = where == null
                ? DbSet
                : DbSet.Where(where);
            var entities = includeProperties.Aggregate(query, (current, includeProperty) =>
                current.Include(includeProperty));


            return entities;
        }
        public TEntity Get<TKey>(TKey id)
        {
            return DbSet.Find(id);
        }
        public void Add(TEntity entity)
        {
            try
            {
                DbContext.Set<TEntity>().Add(entity);

                Save();
            }
            catch (Exception e)
            {
                var s = e.Message;
                throw;
            }

        }
        public void Delete(int Id)
        {

            DbContext.Remove(DbSet.Find(Id));

            Save();
        }

        public void Save()
        {
            DbContext.SaveChanges();
        }
        public void Edit(TEntity entity)
        {



            DbContext.Entry(entity).State = EntityState.Modified;
            DbContext.SaveChanges();



        }
        public void ExecProcidure(string ProcedureName,string parameter)
        {
            try
            {
                DbContext.Set<TEntity>().FromSqlRaw("EXECUTE "+ProcedureName+" {0}" ,parameter).ToList();


            }
            catch (Exception e)
            {
                var s = e.Message;
                throw;
            }
        }

    }
}
