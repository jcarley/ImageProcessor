using Domain.Interfaces;

using MongoDB.Driver;

namespace Infrastructure;

public class MongoUnitOfWork : IUnitOfWork
{
    private readonly IClientSessionHandle _session;

    public MongoUnitOfWork(IMongoClient mongoClient)
    {
        _session = mongoClient.StartSession();
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

    public IContributionRepository ContributionRepository => new MongoContributionRepository(_session);

    public IEventOutboxRepository EventOutboxRepository => new MongoEventOutboxRepository(_session);
}