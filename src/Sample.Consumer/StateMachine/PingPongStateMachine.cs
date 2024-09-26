using MassTransit;
using MassTransit.Initializers;
using Sample.Common.Contract;
using Sample.Common.StateMachine;

namespace Sample.Consumer.StateMachine;

public class PingPongStateMachine : MassTransitStateMachine<PingPong>
{
    public State ProcessingTwo { get; set; } = null!;

    public Event<PongOne> PongOne { get; set; } = null!;
    public Event<PongTwo> PongTwo { get; set; } = null!;

    public Event<Fault<PongOne>> PongOneFault { get; set; } = null!;

    public PingPongStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => PongOne, x => x.CorrelateById(c => c.Message.StarterId));
        Event(() => PongTwo, x => x.CorrelateById(c => c.Message.StarterId));

        Event(() => PongOneFault, x => x.CorrelateById(c => c.Message.Message.StarterId));

        Initially(
            When(PongOne)
                .Then(x =>
                {
                    x.Saga.StarterId = x.Message.StarterId;
                })
                .TransitionTo(ProcessingTwo)
            );

        During(ProcessingTwo,
            When(PongTwo)
                .Finalize(),
            When(PongOneFault)
                .Publish(x => new SomethingFailed($"Message {x.Message.Message.StarterId} failed"))
                .Finalize()
            );
    }
}
