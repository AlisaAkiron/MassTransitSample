using MassTransit;
using Sample.Common.Contract;
using Sample.Common.StateMachine;

namespace Sample.Consumer.StateMachine;

public class OrderingStateMachine : MassTransitStateMachine<OrderingData>
{
    public State CreatingPayment { get; set; } = null!;
    public State Paying { get; set; } = null!;

    public Event<OrderCreated> OrderCreated { get; set; } = null!;
    public Event<PaymentCreated> PaymentCreated { get; set; } = null!;
    public Event<PaymentDone> PaymentDone { get; set; } = null!;
    public Event<PaymentFailed> PaymentFailed { get; set; } = null!;

    public OrderingStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderCreated, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentCreated, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentDone, x => x.CorrelateById(context => context.Message.OrderId));

        Initially(
            When(OrderCreated)
                .Then(c =>
                {
                    c.Saga.OrderId = c.Message.OrderId;
                })
                .TransitionTo(CreatingPayment)
                .Publish(c => new CreatePayment(c.Message.OrderId, c.Message.CustomerId, c.Message.Bill))
            );

        During(CreatingPayment,
            When(PaymentCreated)
                .Then(c =>
                {
                    c.Saga.PaymentId = c.Message.PaymentId;
                })
                .TransitionTo(Paying)
                .Publish(c => new MakePayment(c.Message.OrderId, c.Message.PaymentId))
            );

        During(Paying,
            When(PaymentDone)
                .Finalize(),
            When(PaymentFailed)
                .Then(_ => throw new InvalidOperationException("Payment failed"))
                .Finalize()
            );
    }
}
