using System.Security.Cryptography;
using MassTransit;
using Microsoft.Extensions.Logging;
using Sample.Common.Contract;

namespace Sample.Consumer.Handler;

[AutoConstructor]
public partial class PingTwoHandler : IConsumer<PongOne>
{
    private readonly ILogger<PingTwoHandler> _logger;

    /// <inheritdoc />
    public async Task Consume(ConsumeContext<PongOne> context)
    {
        await Task.Delay(100);

        var retryCount = context.GetRetryCount();
        var retryAttempt = context.GetRetryAttempt();

        _logger.LogInformation("PingTwo, Count: {RetryCount}, Attempt: {RetryAttempt}", retryCount, retryAttempt);

        var rand = RandomNumberGenerator.GetInt32(0, 100);

        switch (rand)
        {
            case < 77:
                await context.Publish(new PongTwo(context.Message.StarterId));
                _logger.LogInformation("PingTwo DONE");
                return;
            case < 99:
                throw new InvalidOperationException($"PingTwo IOE {context.Message.StarterId} Failed");
            default:
                throw new ArgumentException($"PingTwo ARG {context.Message.StarterId} Failed");
        }
    }
}
