using E_Commerce.Infrastructure.Data;
using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrastructure.Repositories
{
    internal class GenericRepository<TEntity, Tkey>(StoreDbContext dbContext)
        : IGenericRepository<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {
        public void Add(TEntity entity) => dbContext.Set<TEntity>().Add(entity);
        public void Delete(TEntity entity) => dbContext.Set<TEntity>().Remove(entity);
        public void Update(TEntity entity) => dbContext.Set<TEntity>().Update(entity);

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
            => await dbContext.Set<TEntity>().ToListAsync();
        public async Task<TEntity?> GetByIdAsync(Tkey id, CancellationToken ct = default)
            => await dbContext.Set<TEntity>().FindAsync(id, ct);
    }
}
