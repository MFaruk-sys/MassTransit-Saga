using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Infrastructure.Messaging;

public static class MassTransitExtensions
{
    public static IServiceCollection AddCustomMassTransit(
        this IServiceCollection services,
        Action<IBusRegistrationConfigurator> configureConsumers)
    {
        services.AddMassTransit(x =>
        {
            // Configure consumers
            configureConsumers(x);

            // Configure RabbitMQ
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq://localhost");

                // Configure the endpoints
                cfg.ConfigureEndpoints(context);

                // Configure retry policy
                cfg.UseMessageRetry(r => r.Intervals(100, 500, 1000));
            });
        });

        return services;
    }
}