using ECommerce.RedisExtensions.Abstract;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ECommerce.RedisExtensions.Core
{
    public class RedisCacheManager : ICacheService
    {
        private readonly IDistributedCache _cache;
        public RedisCacheManager(IDistributedCache cache) => _cache = cache;

        public async Task<T?> GetAsync<T>(string key)
        {
            var cachedData = await _cache.GetStringAsync(key);
            return cachedData == null ? default : JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
            };
            var jsonData = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, jsonData, options);
        }
        public async Task RemoveAsync(string key) => await _cache.RemoveAsync(key);

        public async Task<bool> AnyAsync(string key) => (await _cache.GetAsync(key)) != null;
    }
}
