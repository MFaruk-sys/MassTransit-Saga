using MassTransit;
using Shared.Contracts.Events;
using Shared.Contracts.Persistence;

namespace Shared.Contracts.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentCompleted, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentFailed, x => x.CorrelateById(m => m.Message.OrderId));

        Initially(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.CustomerId = context.Message.CustomerId;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.CreatedAt = context.Message.SubmittedAt;
                })
                .Activity(x => x.OfType<StoreEventActivity<OrderState, OrderSubmitted>>())
                .TransitionTo(Submitted));

        During(Submitted,
            When(PaymentCompleted)
                .Then(context =>
                {
                    context.Saga.PaymentId = context.Message.PaymentId;
                    context.Saga.CompletedAt = context.Message.CompletedAt;
                })
                .Activity(x => x.OfType<StoreEventActivity<OrderState, PaymentCompleted>>())
                .TransitionTo(Completed),
            When(PaymentFailed)
                .Then(context =>
                {
                    context.Saga.PaymentFailureReason = context.Message.Reason;
                })
                .Activity(x => x.OfType<StoreEventActivity<OrderState, PaymentFailed>>())
                .TransitionTo(Failed));
    }

    public State Submitted { get; private set; }
    public State Completed { get; private set; }
    public State Failed { get; private set; }

    public Event<OrderSubmitted> OrderSubmitted { get; private set; }
    public Event<PaymentCompleted> PaymentCompleted { get; private set; }
    public Event<PaymentFailed> PaymentFailed { get; private set; }
}
