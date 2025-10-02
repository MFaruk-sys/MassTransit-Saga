using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Shared.Contracts.Persistence;

public interface IEventStore
{
    Task SaveEventAsync<T>(Guid correlationId, T @event);
}

public class MongoEventStore : IEventStore
{
    private readonly IMongoCollection<EventDocument> _collection;

    public MongoEventStore(IMongoDatabase database)
    {
        _collection = database.GetCollection<EventDocument>("events");
    }

    public Task SaveEventAsync<T>(Guid correlationId, T @event)
    {
        var doc = new EventDocument
        {
            Id = Guid.NewGuid(),
            CorrelationId = correlationId,
            EventType = typeof(T).Name,
            EventData = @event,
            CreatedAt = DateTime.UtcNow
        };

        return _collection.InsertOneAsync(doc);
    }
}

public class EventDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public Guid CorrelationId { get; set; }
    public string EventType { get; set; } = null!;
    public object EventData { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
