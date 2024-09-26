using System.Security.Cryptography;
using MassTransit;
using Microsoft.Extensions.Logging;
using Sample.Common.Contract;

namespace Sample.Consumer.Handler;

[AutoConstructor]
public partial class PingOneHandler : IConsumer<PingOne>
{
    private readonly ILogger<PingOneHandler> _logger;

    /// <inheritdoc />
    public async Task Consume(ConsumeContext<PingOne> context)
    {
        await Task.Delay(100);

        var retryCount = context.GetRetryCount();
        var retryAttempt = context.GetRetryAttempt();

        _logger.LogInformation("PingOne, Count: {RetryCount}, Attempt: {RetryAttempt}", retryCount, retryAttempt);

        var rand = RandomNumberGenerator.GetInt32(0, 100);

        switch (rand)
        {
            case < 77:
                await context.Publish(new PongOne(context.Message.StarterId));
                _logger.LogInformation("PingOne DONE");
                return;
            case < 99:
                throw new InvalidOperationException($"PingOne IOE {context.Message.StarterId} Failed");
            default:
                throw new ArgumentException($"PingOne ARG {context.Message.StarterId} Failed");
        }
    }
}
