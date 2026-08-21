using System;
using System.Threading.Tasks;
using ECommerce.Application.Interfaces.Services;
using RedLockNet;
using RedLockNet.SERedis;

namespace ECommerce.Infrastructure.Services;

public class RedLockService : IDistributedLockService
{
    private readonly IDistributedLockFactory _lockFactory;

    public RedLockService(IDistributedLockFactory lockFactory)
    {
        _lockFactory = lockFactory;
    }

    public async Task<IDisposable?> AcquireLockAsync(string resource, TimeSpan expiryTime, TimeSpan waitTime, TimeSpan retryTime)
    {
        var redLock = await _lockFactory.CreateLockAsync(resource, expiryTime, waitTime, retryTime);
        if (redLock.IsAcquired)
        {
            return redLock;
        }
        
        return null;
    }
}
