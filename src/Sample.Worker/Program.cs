using MassTransit;
using Microsoft.EntityFrameworkCore;
using Sample.Aspire.ServiceDefaults;
using Sample.Common.StateMachine;
using Sample.Consumer;
using Sample.Consumer.StateMachine;
using Sample.Database;
using Sample.Database.Entities;

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
    options.AddSagaStateMachines(assembly);

    options.UseAspireRabbitMq((c, cfg) =>
    {
        cfg.ConfigureEndpoints(c);
    });

    options.AddEntityFrameworkOutbox<SampleDbContext>(o =>
    {
        o.UseBusOutbox();
        o.UsePostgres();
    });

    options.SetEntityFrameworkSagaRepositoryProvider(o =>
    {
        o.UsePostgres();
        o.ExistingDbContext<SampleDbContext>();
    });

    options.AddConfigureEndpointsCallback((c, _, ctx) =>
    {
        ctx.UseEntityFrameworkOutbox<SampleDbContext>(c);

        ctx.PublishFaults = true;

        // ctx.UseDelayedRedelivery(r =>
        // {
        //     r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30));
        // });

        ctx.UseMessageRetry(r =>
        {
            r.Exponential(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(2));
            r.Ignore<ArgumentException>();
        });
    });
});
builder.Services.AddMassTransitObservable();

builder.Services.AddStateObserver<OrderingData, OrderingStateMachine.StateObservable>();
builder.Services.AddEventObserver<OrderingData, OrderingStateMachine.EventObservable>();

builder.Services.AddStateObserver<PingPong, PingPongStateMachine.StateObservable>();
builder.Services.AddEventObserver<PingPong, PingPongStateMachine.EventObservable>();

var host = builder.Build();

await host.RunAsync();
