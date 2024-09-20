using MassTransit;
using Sample.Aspire.ServiceDefaults;
using Sample.Common.StateMachine;
using Sample.Consumer;
using Sample.Consumer.StateMachine;
using Sample.Database;

var builder = Host.CreateApplicationBuilder(args);

builder.AddDefaultServices();
builder.AddSampleDbContext();
builder.Services.AddMassTransit(options =>
{
    options.SetKebabCaseEndpointNameFormatter();

    var assembly = ConsumerAssembly.Current;

    options.AddConsumers(assembly);
    options.AddActivities(assembly);
    options.AddSagas(assembly);

    options.AddSagaStateMachine<OrderingStateMachine, OrderingData>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<SampleDbContext>();
            r.UsePostgres();
        });

    options.UseAspireRabbitMq((c, cfg) =>
    {
        cfg.ConfigureEndpoints(c);
    });

    options.AddEntityFrameworkOutbox<SampleDbContext>(o =>
    {
        o.UsePostgres();
    });
});
builder.Services.AddMassTransitObservable();
builder.Services.AddStateObserver<OrderingData, OrderingStateMachine.StateObservable>();

var host = builder.Build();

await host.RunAsync();
