using MassTransit;

namespace Sample.Common.StateMachine;

public class PingPong : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public string CurrentState { get; set; } = string.Empty;

    public Guid StarterId { get; set; }
}
