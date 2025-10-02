using MassTransit;
using Shared.Contracts.Persistence;
using System;
using System.Threading.Tasks;

namespace Shared.Contracts.Persistence;

public class StoreEventActivity<TState, TMessage> : IStateMachineActivity<TState, TMessage>
    where TState : class, SagaStateMachineInstance
    where TMessage : class
{
    private readonly IEventStore _eventStore;

    public StoreEventActivity(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("store-event");
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<TState, TMessage> context, IBehavior<TState, TMessage> next)
    {
        await _eventStore.SaveEventAsync(context.Saga.CorrelationId, context.Message);
        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<TState, TMessage, TException> context, IBehavior<TState, TMessage> next)
        where TException : Exception
    {
        return next.Faulted(context);
    }
}
