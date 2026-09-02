using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Interfaces.Services;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Messaging;
using ECommerce.Infrastructure.Repositories;
using MassTransit;
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
        services.AddScoped<IInventoryService, InventoryService>();

        // Auth Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

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

        // Messaging & Notifications
        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddScoped<IEmailNotificationService, EmailNotificationService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<DeductStockOnOrderPlacedConsumer>();
            x.AddConsumer<SendEmailOnOrderPlacedConsumer>();

            x.AddEntityFrameworkOutbox<AppDbContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            x.SetKebabCaseEndpointNameFormatter();

            var brokerProvider = configuration["MessageBroker:Provider"] ?? "InMemory";

            if (brokerProvider == "AmazonSqs")
            {
                x.UsingAmazonSqs((context, cfg) =>
                {
                    cfg.Host(configuration["AWS:Region"] ?? "us-east-1", h =>
                    {
                        h.AccessKey(configuration["AWS:AccessKey"]);
                        h.SecretKey(configuration["AWS:SecretKey"]);
                    });
                    cfg.ConfigureEndpoints(context);
                });
            }
            else if (brokerProvider == "RabbitMq")
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["MessageBroker:RabbitMq:Host"] ?? "localhost", h =>
                    {
                        h.Username(configuration["MessageBroker:RabbitMq:Username"] ?? "guest");
                        h.Password(configuration["MessageBroker:RabbitMq:Password"] ?? "guest");
                    });
                    cfg.ConfigureEndpoints(context);
                });
            }
            else
            {
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            }
        });

        return services;
    }
}

