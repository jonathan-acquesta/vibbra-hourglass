using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Vibbra.Hourglass.Domain.Base;
using Vibbra.Hourglass.Infra.Context;
using Vibbra.Hourglass.Infra.Interfaces;

namespace Vibbra.Hourglass.Infra.Repository
{
    public class ApplicationRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseDomain
    {
        #region Fields
        protected DbContext DbContext;
        #endregion

        #region Constructor
        public ApplicationRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }
        #endregion

        #region Methods
        public async Task<TEntity> Insert(TEntity obj)
        {
            obj.CreatedDate = DateTime.Now;
            var user = await DbContext.Set<TEntity>().AddAsync(obj);
            return user.Entity;
        }


        public async Task BulkInsert(IEnumerable<TEntity> objs)
        {
            foreach (var obj in objs)
            {
                obj.CreatedDate = DateTime.Now;
            }

            DbContext.AddRange(objs);

        }

        public async Task BulkUpdate(IEnumerable<TEntity> objs)
        {
            foreach (var obj in objs)
            {
                obj.UpdatedDate = DateTime.Now;
            }

            DbContext.UpdateRange(objs);

        }

        public async Task<TEntity> SelectFirstBy(Expression<Func<TEntity, bool>> predicate) =>
           await DbContext.Set<TEntity>().Where(x => x.DeletedAt == null).FirstOrDefaultAsync(predicate);

        public async Task<IList<TEntity>> Select() =>
            await DbContext.Set<TEntity>().Where(x => x.DeletedAt == null).ToListAsync();

        public async Task<IList<TEntity>> Select(params string[] relations)
        {
            var query = DbContext.Set<TEntity>().Where(x => x.DeletedAt == null);

            foreach (var relation in relations)
            {
                query = query.Include(relation);
            }

            return await query.ToListAsync();
        }

        public async Task<IList<TEntity>> Select(Expression<Func<TEntity, bool>> predicate) =>
          await DbContext.Set<TEntity>().Where(predicate).Where(x => x.DeletedAt == null).ToListAsync();

        public async Task<IList<TEntity>> SelectTopOrderByAsc<TKey>(int top, Expression<Func<TEntity, TKey>> orderBy) =>
            await DbContext.Set<TEntity>().Where(x => x.DeletedAt == null).OrderBy(orderBy).Take(top).ToListAsync();

        public async Task<IList<TEntity>> SelectTopOrderByDesc<TKey>(int top, Expression<Func<TEntity, TKey>> orderBy) =>
            await DbContext.Set<TEntity>().Where(x => x.DeletedAt == null).OrderByDescending(orderBy).Take(top).ToListAsync();


        public async Task<IList<TEntity>> Select(Expression<Func<TEntity, bool>> predicate, params string[] relations)
        {
            var query = DbContext.Set<TEntity>().Where(predicate).Where(x => x.DeletedAt == null);

            foreach (var relation in relations)
            {
                query = query.Include(relation);
            }

            return await query.ToListAsync();
        }

        public async Task<TEntity> DeleteFirstBy(Expression<Func<TEntity, bool>> predicate)
        {
            var data = await SelectFirstBy(predicate);
            data.UpdatedDate = DateTime.Now;
            data.DeletedAt = DateTime.Now;
            DbContext.Entry(data).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();

            return data;
        }

        public async Task PhysicalDelete(TEntity obj)
        {
            DbContext.Remove(obj);
            await DbContext.SaveChangesAsync();
        }

        public async Task Update(TEntity obj)
        {
            obj.UpdatedDate = DateTime.Now;
            DbContext.Entry(obj).State = EntityState.Modified;
        }

        public async Task<TEntity> SelectFirstBy(Expression<Func<TEntity, bool>> predicate, params string[] relations)
        {
            var query = DbContext.Set<TEntity>().Where(x => x.DeletedAt == null).Where(predicate);

            foreach (var relation in relations)
            {
                query = query.Include(relation);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<bool> CommitAsync()
        {
            return await DbContext.SaveChangesAsync() > 0;
        }

        public Task Rollback()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}
