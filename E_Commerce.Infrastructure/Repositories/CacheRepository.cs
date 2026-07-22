using E_Commerce.Domain.Contracts;
using StackExchange.Redis;

namespace E_Commerce.Infrastructure.Repositories
{
    internal class CacheRepository : ICacheRepository
    {
        private readonly IDatabase _database;
        public CacheRepository(IConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();
        }
        public async Task<string?> GetAsync(string cacheKey, CancellationToken ct = default)
        {
            var Value = await _database.StringGetAsync(cacheKey);
            return Value.IsNullOrEmpty ? null : Value.ToString();
        }

        public async Task SetAsync(string cacheKey, string cacheValue, TimeSpan? timeToLive = null, CancellationToken ct = default)
        {
            await _database.StringSetAsync(cacheKey, cacheValue, timeToLive ?? TimeSpan.FromDays(2));
        }
    }
}
