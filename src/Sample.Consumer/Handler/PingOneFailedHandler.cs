using MassTransit;
using Microsoft.Extensions.Logging;
using Sample.Common.Contract;

namespace Sample.Consumer.Handler;

[AutoConstructor]
public partial class PingOneFailedHandler : IConsumer<Fault<PingOne>>
{
    private readonly ILogger<PingOneFailedHandler> _logger;

    /// <inheritdoc />
    public Task Consume(ConsumeContext<Fault<PingOne>> context)
    {
        _logger.LogInformation("PingOneFailedHandler, Faulted: {FaultedMessage}", context.Message.Message.StarterId);

        return Task.CompletedTask;
    }
}
