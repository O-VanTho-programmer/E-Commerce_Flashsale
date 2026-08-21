using System;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Services;
using StackExchange.Redis;

namespace ECommerce.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var db = _redis.GetDatabase();
        var value = await db.StringGetAsync(key);

        if (!value.HasValue)
            return default;

        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
    {
        var db = _redis.GetDatabase();
        var serializedValue = JsonSerializer.Serialize(value);
        
        if (absoluteExpireTime.HasValue)
        {
            await db.StringSetAsync(key, serializedValue, absoluteExpireTime.Value);
        }
        else
        {
            await db.StringSetAsync(key, serializedValue);
        }
    }

    public async Task RemoveAsync(string key)
    {
        var db = _redis.GetDatabase();
        await db.KeyDeleteAsync(key);
    }
}
