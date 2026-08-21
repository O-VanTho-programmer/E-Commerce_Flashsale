using System;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services;

public interface IDistributedLockService
{
    Task<IDisposable?> AcquireLockAsync(string resource, TimeSpan expiryTime, TimeSpan waitTime, TimeSpan retryTime);
}
