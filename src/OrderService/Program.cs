using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using Shared.Contracts.Events;
using Shared.Contracts.StateMachines;
using Shared.Infrastructure.Logging;
using Shared.Infrastructure.Messaging;
using Shared.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom logging
builder.Services.AddCustomLogging();

// Add MassTransit with State Machine
builder.Services.AddCustomMassTransit(x =>
{
    // No consumers to add in OrderService for now
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use custom exception handling
app.UseCustomExceptionHandler();

// Endpoint to submit an order
app.MapPost("/orders", async (
    [FromBody] SubmitOrderRequest request,
    [FromServices] IPublishEndpoint publishEndpoint,
    [FromServices] ILogger<Program> logger) =>
{
    var orderId = Guid.NewGuid();
    logger.LogInformation("Submitting order {OrderId} for customer {CustomerId}", orderId, request.CustomerId);

    var orderSubmitted = new OrderSubmitted(
        orderId,
        request.Amount,
        request.CustomerId,
        DateTime.UtcNow
    );

    await publishEndpoint.Publish(orderSubmitted);

    logger.LogInformation("Published OrderSubmitted event for order {OrderId}", orderId);

    return Results.Ok(new { OrderId = orderId });
})
.WithName("SubmitOrder")
.WithOpenApi();

app.Run();
