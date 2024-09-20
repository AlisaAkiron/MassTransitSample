using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OpenTelemetry.Trace;
using Sample.Database;
using Sample.Database.Entities;

namespace Sample.Migrator;


public class Worker : BackgroundService
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    private const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public Worker(
        IHostEnvironment hostEnvironment,
        IServiceProvider serviceProvider,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _hostEnvironment = hostEnvironment;
        _serviceProvider = serviceProvider;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // ReSharper disable once ExplicitCallerInfoArgument
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SampleDbContext>();

            await EnsureDatabaseAsync(dbContext, stoppingToken);
            await RunMigrationAsync(dbContext, stoppingToken);

            if (_hostEnvironment.IsDevelopment())
            {
                await SeedDevelopmentDataAsync(dbContext, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }

        _hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(SampleDbContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            if (await dbCreator.ExistsAsync(cancellationToken) is false)
            {
                await dbCreator.CreateAsync(cancellationToken);
            }
        });
    }

    private static async Task RunMigrationAsync(SampleDbContext dbContext, CancellationToken cancellationToken)
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pendingMigrations.Any() is false)
        {
            return;
        }

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            await dbContext.Database.MigrateAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }

    private static async Task SeedDevelopmentDataAsync(SampleDbContext dbContext, CancellationToken cancellationToken)
    {
        var seedCustomer = await dbContext.Customers.AnyAsync(cancellationToken) is false;
        var seedProduct = await dbContext.Products.AnyAsync(cancellationToken) is false;

        if (seedCustomer)
        {
            await dbContext.Customers.AddRangeAsync([
                new Customer
                {
                    Id = 1,
                    Name = "John Doe",
                    Balance = 100.00m
                },
                new Customer
                {
                    Id = 2,
                    Name = "Jane Doe",
                    Balance = 2000.00m
                },
                new Customer
                {
                    Id = 3,
                    Name = "Alice",
                    Balance = 30000.00m
                }
            ], cancellationToken);
        }

        if (seedProduct)
        {
            await dbContext.Products.AddRangeAsync([
                new Product
                {
                    Id = 1,
                    Name = "Product 1",
                    Price = 100.00m
                },
                new Product
                {
                    Id = 2,
                    Name = "Product 2",
                    Price = 200.00m
                },
                new Product
                {
                    Id = 3,
                    Name = "Product 3",
                    Price = 300.00m
                }
            ], cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
