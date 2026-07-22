using E_Commerce.Application.Contracts;
using E_Commerce.Domain.Contracts;
using System.Text.Json;

namespace E_Commerce.Application.Services
{
    internal class CacheService : ICacheService
    {
        private readonly ICacheRepository _cacheRepository;

        public CacheService(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }
        public async Task<string?> GetDataAsync(string cacheKey, CancellationToken ct = default)
            => await _cacheRepository.GetAsync(cacheKey, ct);

        public async Task SetDataAsync(string cacheKey, object cacheValue, TimeSpan? timeToLive = null, CancellationToken ct = default)
        {
            var Value = JsonSerializer.Serialize(cacheValue,new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await _cacheRepository.SetAsync(cacheKey,Value, timeToLive, ct);
        }
    }
}
