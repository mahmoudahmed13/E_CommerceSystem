using E_Commerce.Infrastructure.Data;
using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;

namespace E_Commerce.Infrastructure.Repositories
{
    internal class UnitOfWork(StoreDbContext dbContext) : IUnitOfWork
    {
        private readonly Dictionary<string, object> _repositories = [];

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var entityType = typeof(TEntity).Name;
            if (_repositories.TryGetValue(entityType, out var value))
                return (IGenericRepository<TEntity, TKey>)value;
            else
            {
                var repo = new GenericRepository<TEntity, TKey>(dbContext);
                _repositories[entityType] = repo;
                return repo;
            }
        }
        public Task<int> SaveChangesAsync(CancellationToken ct = default) => dbContext.SaveChangesAsync(ct);
    }
}
