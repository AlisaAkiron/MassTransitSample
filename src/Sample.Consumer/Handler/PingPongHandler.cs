using MassTransit;
using Sample.Common.Contract;

namespace Sample.Consumer.Handler;

public class PingPongHandler : IConsumer<Ping>
{
    public async Task Consume(ConsumeContext<Ping> context)
    {
        await context.Publish(new Pong());
    }
}
