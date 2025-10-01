namespace Shared.Contracts.Events;

public record OrderSubmitted(
    Guid OrderId,
    decimal Amount,
    string CustomerId,
    DateTime SubmittedAt
);

public record PaymentCompleted(
    Guid OrderId,
    Guid PaymentId,
    decimal Amount,
    DateTime CompletedAt
);

public record PaymentFailed(
    Guid OrderId,
    string Reason,
    DateTime FailedAt
);