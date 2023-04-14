using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Vibbra.Hourglass.Domain.Base;

namespace Vibbra.Hourglass.Infra.Interfaces
{
    public interface IBaseRepository<TEntity> : IUnitOfWork where TEntity : BaseDomain
    {
        Task<TEntity> Insert(TEntity obj);

        Task<TEntity> SelectFirstBy(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> SelectFirstBy(Expression<Func<TEntity, bool>> predicate, params string[] relations);

        Task BulkInsert(IEnumerable<TEntity> objs);

        Task BulkUpdate(IEnumerable<TEntity> objs);

        Task<IList<TEntity>> Select();

        Task<IList<TEntity>> Select(params string[] relations);

        Task<IList<TEntity>> Select(Expression<Func<TEntity, bool>> predicate);

        Task<IList<TEntity>> Select(Expression<Func<TEntity, bool>> predicate, params string[] relations);

        Task<TEntity> DeleteFirstBy(Expression<Func<TEntity, bool>> predicate);

        Task<IList<TEntity>> SelectTopOrderByAsc<TKey>(int top, Expression<Func<TEntity, TKey>> orderBy);

        Task<IList<TEntity>> SelectTopOrderByDesc<TKey>(int top, Expression<Func<TEntity, TKey>> orderBy);

        Task PhysicalDelete(TEntity obj);

        Task Update(TEntity obj);
    }
}
