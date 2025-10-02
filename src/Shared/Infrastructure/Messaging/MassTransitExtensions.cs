using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts.StateMachines;
using MongoDB.Driver;
using Shared.Contracts.Persistence;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Shared.Infrastructure.Messaging;

public static class MassTransitExtensions
{
    public static IServiceCollection AddCustomMassTransit(
        this IServiceCollection services,
        Action<IBusRegistrationConfigurator> configureConsumers)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || type.FullName.StartsWith("Shared.Contracts")));
        services.AddSingleton<IMongoClient>(sp => new MongoClient("mongodb://localhost:27017"));
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase("saga");
        });
        services.AddSingleton<IEventStore, MongoEventStore>();
        services.AddMassTransit(x =>
        {
            x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                .MongoDbRepository(r =>
                {
                    r.Connection = "mongodb://localhost:27017";
                    r.DatabaseName = "saga";
                });

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
