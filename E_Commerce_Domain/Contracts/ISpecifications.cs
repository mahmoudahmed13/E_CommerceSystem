using E_Commerce.Domain.Common;
using System.Linq.Expressions;

namespace E_Commerce.Domain.Contracts
{
    public interface ISpecifications<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        ICollection<Expression<Func<TEntity, object>>> IncludeExpressions { get; }
        Expression<Func<TEntity, bool>> Criteria { get; }
        Expression<Func<TEntity, object>>? OrderBy { get; }
        Expression<Func<TEntity, object>>? OrderByDesending { get; }
        public int Take { get; }
        public int Skip { get; }

        public bool IsPaginated { get; }
    }
}
