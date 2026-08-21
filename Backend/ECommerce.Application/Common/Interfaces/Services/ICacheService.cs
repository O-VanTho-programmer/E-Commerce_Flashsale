using System;
using System.Threading.Tasks;

namespace ECommerce.Application.Common.Interfaces.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null);
    Task RemoveAsync(string key);
}
