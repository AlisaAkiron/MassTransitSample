using MassTransit;
using MassTransit.Observables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Aspire.ServiceDefaults;

public static partial class Extensions
{
    public static void UseAspireRabbitMq(this IBusRegistrationConfigurator options, Action<IBusRegistrationContext,IRabbitMqBusFactoryConfigurator>? configure = null)
    {
        options.UsingRabbitMq((c, cfg) =>
        {
            var configuration = c.GetRequiredService<IConfiguration>();
            var rabbitMqConnectionString = configuration.GetConnectionString("RabbitMQ");
            cfg.Host(rabbitMqConnectionString);

            configure?.Invoke(c, cfg);
        });
    }

    public static void AddMassTransitObservable(this IServiceCollection services)
    {
        services.AddBusObserver<BusObservable>();
        services.AddReceiveEndpointObserver<ReceiveEndpointObservable>();
        services.AddReceiveObserver<ReceiveObservable>();
        services.AddConsumeObserver<ConsumeObservable>();
        services.AddSendObserver<SendObservable>();
        services.AddPublishObserver<PublishObservable>();
    }
}
