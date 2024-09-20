using MassTransit;

namespace Sample.Common.StateMachine;

public class OrderingData : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public string CurrentState { get; set; } = string.Empty;

    public Guid OrderId { get; set; }
    public Guid PaymentId { get; set; }

    // Status

    public bool Paid { get; set; }
}
