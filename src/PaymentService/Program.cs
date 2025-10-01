using PaymentService.Consumers;
using Shared.Infrastructure.Logging;
using Shared.Infrastructure.Messaging;
using Shared.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom logging
builder.Services.AddCustomLogging();

// Add MassTransit with consumers
builder.Services.AddCustomMassTransit(x =>
{
    x.AddConsumer<OrderSubmittedConsumer>();
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

app.Run();
