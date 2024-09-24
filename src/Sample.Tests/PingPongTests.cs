using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Sample.Common.Contract;
using Sample.Consumer.Handler;

namespace Sample.Tests;

public class PingPongTests
{
    [Test]
    public async Task PingPongHandler()
    {
        var sp = new ServiceCollection()
            .AddMassTransitTestHarness(options =>
            {
                options.AddConsumer<PingPongHandler>();
            })
            .BuildServiceProvider(true);

        var harness = sp.GetRequiredService<ITestHarness>();

        await harness.Start();

        await harness.Bus.Publish(new Ping());

        await Assert.That(async () => await harness.Published.Any<Ping>()).IsTrue();

        var consumerHarness = harness.GetConsumerHarness<PingPongHandler>();

        await Assert.That(async () => await consumerHarness.Consumed.Any<Ping>()).IsTrue();

        await Assert.That(async () => await harness.Published.Any<Pong>()).IsTrue();
    }
}
