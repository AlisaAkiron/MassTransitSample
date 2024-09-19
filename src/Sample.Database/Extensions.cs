using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OpenTelemetry.Metrics;

namespace Sample.Database;

public static class Extensions
{
    public static void AddSampleDbContext(this IHostApplicationBuilder builder)
    {
        var postgresqlConnectionString = builder.Configuration.GetConnectionString("PostgreSQL");
        builder.Services.AddDbContext<SampleDbContext>(options =>
        {
            options.UseNpgsql(postgresqlConnectionString);
        });

        builder.Services
            .AddHealthChecks()
            .AddDbContextCheck<SampleDbContext>();

        double[] secondsBuckets = [0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10];

        builder.Services
            .AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter("Npgsql")
                    .AddView("db.client.commands.duration",
                        new ExplicitBucketHistogramConfiguration
                        {
                            Boundaries = secondsBuckets
                        })
                    .AddView("db.client.connections.create_time",
                        new ExplicitBucketHistogramConfiguration
                        {
                            Boundaries = secondsBuckets
                        });
            })
            .WithTracing(tracer =>
            {
                tracer.AddNpgsql();
            });
    }
}
