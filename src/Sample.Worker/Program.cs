using MassTransit;
using Sample.Aspire.ServiceDefaults;
using Sample.Database;

var builder = Host.CreateApplicationBuilder(args);

builder.AddDefaultServices();
builder.AddSampleDbContext();
builder.Services.AddMassTransit(options =>
{
    options.SetKebabCaseEndpointNameFormatter();

    var assembly = typeof(Program).Assembly;
    options.AddConsumers(assembly);
    options.AddActivities(assembly);
    options.AddSagaStateMachines(assembly);
    options.AddSagas(assembly);

    options.UseAspireRabbitMq((c, cfg) =>
    {
        cfg.ConfigureEndpoints(c);
    });

    options.AddEntityFrameworkOutbox<SampleDbContext>(o =>
    {
        o.UsePostgres();
    });
});

var host = builder.Build();

await host.RunAsync();
