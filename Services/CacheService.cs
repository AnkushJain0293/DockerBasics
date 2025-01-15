using StackExchange.Redis;
using System.Text.Json;

namespace TrainingService.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _redisCache;

        public CacheService(IConnectionMultiplexer redis)
        {
            _redisCache = redis.GetDatabase();
        }
        public async Task<T> GetData<T>(string key)
        {
            var cachedData = await _redisCache.StringGetAsync(key);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<T>(cachedData);
            }
            return default(T);
        }

        public async Task<bool> SetData<T>(string key, T value, TimeSpan expirationTime)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
            return await _redisCache.StringSetAsync(key, JsonSerializer.Serialize(value, options), expirationTime);
        }
    }
}
