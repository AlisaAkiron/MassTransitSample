using MassTransit;
using Sample.Contract.Models;

namespace Sample.Worker.Consumer;

[AutoConstructor]
public partial class PingReceivedConsumer : IConsumer<PingReceivedEvent>
{
    private readonly ILogger<PingReceivedConsumer> _logger;

    public Task Consume(ConsumeContext<PingReceivedEvent> context)
    {
        _logger.LogInformation("Ping received from {ConnectionId}", context.Message.ConnectionId);

        return Task.CompletedTask;
    }
}
