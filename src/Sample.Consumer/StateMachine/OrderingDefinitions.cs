using MassTransit;
using Sample.Common.StateMachine;
using Sample.Database;

namespace Sample.Consumer.StateMachine;

public class OrderingDefinitions : SagaDefinition<OrderingData>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderingData> sagaConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 1000));

        endpointConfigurator.UseEntityFrameworkOutbox<SampleDbContext>(context);
    }
}
