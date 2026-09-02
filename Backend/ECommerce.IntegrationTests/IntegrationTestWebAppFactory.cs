using Microsoft.Extensions.Configuration;
using ECommerce.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;
using Testcontainers.Redis;
using Xunit;

namespace ECommerce.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.0")
        .Build();

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var redisConn = _redisContainer.GetConnectionString() + ",abortConnect=false";
        
        builder.UseSetting("ConnectionStrings:DefaultConnection", _msSqlContainer.GetConnectionString());
        builder.UseSetting("Redis:Configuration", redisConn);

        builder.ConfigureTestServices(services =>
        {
            // Ensure DB is created & Migrated
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();

            // Add MassTransit Test Harness (overrides actual transport with InMemory for testing)
            services.AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ECommerce.Infrastructure.Messaging.DeductStockOnOrderPlacedConsumer>();
                x.AddConsumer<ECommerce.Infrastructure.Messaging.SendEmailOnOrderPlacedConsumer>();
            });
        });
    }

    public new async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync().AsTask();
        await _redisContainer.DisposeAsync().AsTask();
    }
}
