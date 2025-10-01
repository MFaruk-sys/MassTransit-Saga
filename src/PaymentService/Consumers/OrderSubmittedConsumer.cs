using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;

namespace PaymentService.Consumers;

public class OrderSubmittedConsumer : IConsumer<OrderSubmitted>
{
    private readonly ILogger<OrderSubmittedConsumer> _logger;

    public OrderSubmittedConsumer(ILogger<OrderSubmittedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderSubmitted> context)
    {
        var orderSubmitted = context.Message;
        _logger.LogInformation("Processing payment for order {OrderId}", orderSubmitted.OrderId);

        try
        {
            // Simulate payment processing
            await Task.Delay(1000);

            // Randomly succeed or fail payment (70% success rate)
            if (Random.Shared.NextDouble() < 0.7)
            {
                var paymentCompleted = new PaymentCompleted(
                    orderSubmitted.OrderId,
                    Guid.NewGuid(),
                    orderSubmitted.Amount,
                    DateTime.UtcNow
                );

                await context.Publish(paymentCompleted);
                _logger.LogInformation("Payment completed for order {OrderId}", orderSubmitted.OrderId);
            }
            else
            {
                var paymentFailed = new PaymentFailed(
                    orderSubmitted.OrderId,
                    "Insufficient funds",
                    DateTime.UtcNow
                );

                await context.Publish(paymentFailed);
                _logger.LogWarning("Payment failed for order {OrderId}: {Reason}", orderSubmitted.OrderId, paymentFailed.Reason);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for order {OrderId}", orderSubmitted.OrderId);
            throw;
        }
    }
}