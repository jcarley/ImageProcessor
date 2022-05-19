using System.Diagnostics;

using Domain.Interfaces;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace Infrastructure;

public class MongoUnitOfWork : IUnitOfWork
{
    private readonly string _dbName;
    private readonly IClientSessionHandle _session;

    public MongoUnitOfWork(IOptions<MongoDBSettings> mongoDbSettings)
    {
        Debug.Assert(mongoDbSettings.Value.MongoDbDatabaseName != null,
            "mongoDbSettings.Value.MongoDbDatabaseName != null");

        _dbName = mongoDbSettings.Value.MongoDbDatabaseName;

        MongoClient client = new(mongoDbSettings.Value.ConnectionString());

        _session = client.StartSession();
    }

    public ITransaction BeginTransaction()
    {
        if (!_session.IsInTransaction)
        {
            _session.StartTransaction();
        }

        ITransaction transaction = new MongoDbTransaction(_session);
        transaction.AutoCommit = true;

        return transaction;
    }

    public ITransaction BeginTransaction(IEventPublisher publisher)
    {
        ITransaction transaction = BeginTransaction();
        publisher.Transaction.Value = transaction;
        return transaction;
    }

    public IContributionRepository ContributionRepository => new MongoContributionRepository(_dbName, _session);

    public IEventOutboxRepository EventOutboxRepository => new MongoEventOutboxRepository(_dbName, _session);
}