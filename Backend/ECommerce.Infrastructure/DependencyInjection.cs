using System;
using System.Collections.Generic;
using System.Net;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Interfaces.Services;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Redis
        var redisConfig = configuration.GetSection("Redis")["Configuration"];
        if (!string.IsNullOrEmpty(redisConfig))
        {
            var multiplexer = ConnectionMultiplexer.Connect(redisConfig);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            services.AddScoped<ICacheService, RedisCacheService>();

            // RedLock
            var endPoints = new List<RedLockMultiplexer>
            {
                new RedLockMultiplexer(multiplexer)
            };
            var redLockFactory = RedLockFactory.Create(endPoints);
            services.AddSingleton<IDistributedLockFactory>(redLockFactory);
            services.AddScoped<IDistributedLockService, RedLockService>();
        }

        return services;
    }
}
