using MassTransit;

namespace Shared.Contracts.StateMachines;

public class OrderState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public string? CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentFailureReason { get; set; }
    public Guid? PaymentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int Version { get; set; }
}