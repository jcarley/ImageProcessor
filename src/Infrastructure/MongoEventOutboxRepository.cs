using Domain.Entities;
using Domain.Interfaces;

using MongoDB.Driver;

namespace Infrastructure;

public class MongoEventOutboxRepository : MongoBaseRepository<OutBoxEvent>, IEventOutboxRepository
{
    public MongoEventOutboxRepository(string dbName, IClientSessionHandle clientSessionHandle)
        : base(clientSessionHandle)
    {
        DbName = dbName;

        // PipelineDefinition<ChangeStreamDocument<OutBoxEvent>, ChangeStreamDocument<OutBoxEvent>>? pipeline =
        //     new EmptyPipelineDefinition<ChangeStreamDocument<OutBoxEvent>>()
        //         .Match(x => x.OperationType == ChangeStreamOperationType.Insert);
        //
        // ChangeStreamOptions changeStreamOptions = new()
        // {
        //     BatchSize = 25, FullDocument = ChangeStreamFullDocumentOption.Default,
        // };
        //
        // IChangeStreamCursor<ChangeStreamDocument<OutBoxEvent>>? cursor = clientSessionHandle
        //     .Client
        //     .GetDatabase(DbName)
        //     .GetCollection<OutBoxEvent>(CollectionName)
        //     .Watch(pipeline, changeStreamOptions);
        //
        // foreach (ChangeStreamDocument<OutBoxEvent>? change in cursor.ToEnumerable())
        // {
        //     // do something with change event
        //
        //     OutBoxEvent? outBoxEvent = change.FullDocument;
        // }
    }

    protected override string DbName { get; }

    protected override string CollectionName => "event_outbox";
}