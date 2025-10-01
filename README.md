# MassTransit Saga Example

This project demonstrates a distributed saga pattern using MassTransit and RabbitMQ in a .NET 8 microservices architecture.

## Project Structure

```
src/
├── OrderService/          # Handles order submission
├── PaymentService/        # Processes payments
└── Shared/
    ├── Contracts/        # Shared types and state machines
    └── Infrastructure/   # Shared infrastructure components
```

## Services

### OrderService
- Minimal API for order submission
- Publishes `OrderSubmitted` events
- Manages order state using saga pattern

### PaymentService
- Consumes `OrderSubmitted` events
- Processes payments (simulated)
- Publishes `PaymentCompleted` or `PaymentFailed` events

## Technology Stack

- .NET 8
- MassTransit
- RabbitMQ
- Minimal APIs
- Structured Logging

## Features

- Event-driven architecture
- Saga state machine for order processing
- Clean separation of concerns
- Structured logging
- Exception handling middleware
- InMemory saga repository (can be replaced with persistent storage)

## Prerequisites

- .NET 8 SDK
- RabbitMQ Server
- Docker (optional, for RabbitMQ container)

## Getting Started

1. Start RabbitMQ:
   ```bash
   docker run -d --hostname my-rabbit --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
   ```

2. Run the OrderService:
   ```bash
   cd src/OrderService
   dotnet run
   ```

3. Run the PaymentService:
   ```bash
   cd src/PaymentService
   dotnet run
   ```

4. Submit an order (using Swagger UI or curl):
   ```bash
   curl -X POST http://localhost:5000/orders \
   -H "Content-Type: application/json" \
   -d '{"customerId": "customer123", "amount": 100.00}'
   ```

## Architecture

The system uses a saga pattern to coordinate the order processing workflow:

1. OrderService receives order submission
2. OrderSubmitted event is published
3. PaymentService processes the payment
4. Payment result event is published (success/failure)
5. Saga updates order state accordingly

## State Machine

The order saga has three states:
- Submitted: Initial state after order submission
- Completed: Payment successful
- Failed: Payment failed