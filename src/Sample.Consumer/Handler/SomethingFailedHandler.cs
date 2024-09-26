using MassTransit;
using Microsoft.Extensions.Logging;
using Sample.Common.Contract;

namespace Sample.Consumer.Handler;

[AutoConstructor]
public partial class SomethingFailedHandler : IConsumer<SomethingFailed>
{
    private readonly ILogger<SomethingFailedHandler> _logger;

    /// <inheritdoc />
    public Task Consume(ConsumeContext<SomethingFailed> context)
    {
        _logger.LogError("Something failed: {Message}", context.Message.Message);

        return Task.CompletedTask;
    }
}
