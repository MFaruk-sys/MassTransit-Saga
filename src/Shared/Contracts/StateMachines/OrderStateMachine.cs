using MassTransit;
using Shared.Contracts.Events;
using Shared.Contracts.Persistence;
using Microsoft.Extensions.DependencyInjection;

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
                .ThenAsync(async context =>
                {
                    context.Saga.CustomerId = context.Message.CustomerId;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.CreatedAt = context.Message.SubmittedAt;

                    var serviceProvider = context.GetPayload<IServiceProvider>();
                    var store = serviceProvider.GetRequiredService<IEventStore>();
                    await store.SaveEventAsync(context.Saga.CorrelationId, context.Message);
                })
                .TransitionTo(Submitted));

        During(Submitted,
            When(PaymentCompleted)
                .ThenAsync(async context =>
                {
                    context.Saga.PaymentId = context.Message.PaymentId;
                    context.Saga.CompletedAt = context.Message.CompletedAt;

                    var serviceProvider = context.GetPayload<IServiceProvider>();
                    var store = serviceProvider.GetRequiredService<IEventStore>();
                    await store.SaveEventAsync(context.Saga.CorrelationId, context.Message);
                })
                .TransitionTo(Completed),
            When(PaymentFailed)
                .ThenAsync(async context =>
                {
                    context.Saga.PaymentFailureReason = context.Message.Reason;

                    var serviceProvider = context.GetPayload<IServiceProvider>();
                    var store = serviceProvider.GetRequiredService<IEventStore>();
                    await store.SaveEventAsync(context.Saga.CorrelationId, context.Message);
                })
                .TransitionTo(Failed));
    }

    public State Submitted { get; private set; }
    public State Completed { get; private set; }
    public State Failed { get; private set; }

    public Event<OrderSubmitted> OrderSubmitted { get; private set; }
    public Event<PaymentCompleted> PaymentCompleted { get; private set; }
    public Event<PaymentFailed> PaymentFailed { get; private set; }
}