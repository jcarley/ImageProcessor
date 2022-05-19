using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace Infrastructure;

public class MongoDbTransactionalPublisher : IEventPublisher
{
    public MongoDbTransactionalPublisher(IOptions<MongoDBSettings> mongoDbSettings)
    {
        DbName = mongoDbSettings.Value.MongoDbDatabaseName;
        Transaction = new AsyncLocal<ITransaction>();
    }

    private string DbName { get; }
    private string CollectionName => "outbox.published";

    public AsyncLocal<ITransaction> Transaction { get; set; }

    public async Task Publish(string streamName, EventBase evt)
    {
        OutBoxEvent outboxEvent = new()
        {
            Id = Guid.NewGuid(),
            Retries = 0,
            CreatedAt = DateTime.Now,
            OutputStream = streamName,
            SerializedValue = evt,
        };

        if (Transaction.Value != null)
        {
            IClientSessionHandle? session = Transaction.Value.DbTransaction as IClientSessionHandle;

            if (session != null)
            {
                IMongoDatabase? db = session.Client.GetDatabase(DbName);

                IMongoCollection<OutBoxEvent>? coll = db.GetCollection<OutBoxEvent>(CollectionName);

                InsertOneOptions options = new() { BypassDocumentValidation = false };

                await coll.InsertOneAsync(outboxEvent, options);
            }
        }
    }
}