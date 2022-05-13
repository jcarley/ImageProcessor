using Domain.Interfaces;

using MongoDB.Driver;

namespace Infrastructure;

public class MongoDbTransaction : ITransaction
{
    public MongoDbTransaction(IClientSessionHandle session)
    {
        DbTransaction = session;
    }

    public void Dispose()
    {
        (DbTransaction as IClientSessionHandle)?.Dispose();
        DbTransaction = null;
    }

    public bool AutoCommit { get; set; }

    public object? DbTransaction { get; set; }

    public void Commit()
    {
        if (DbTransaction is IClientSessionHandle session)
        {
            session.CommitTransaction();
        }
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (DbTransaction is IClientSessionHandle session)
        {
            await session.CommitTransactionAsync(cancellationToken);
        }
    }

    public void Rollback()
    {
        if (DbTransaction is IClientSessionHandle session)
        {
            session.AbortTransaction();
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (DbTransaction is IClientSessionHandle session)
        {
            await session.AbortTransactionAsync(cancellationToken);
        }
    }
}

public class MongoUnitOfWork : IUnitOfWork, IAmTransactional
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

    // public async Task<TResult> WithTransaction<TResult>(Func<IUnitOfWork, Task<TResult>> callback)
    // {
    //     TResult result = default;
    //     if (!_session.IsInTransaction)
    //     {
    //         try
    //         {
    //             _session.StartTransaction();
    //             result = await callback(this);
    //             await _session.CommitTransactionAsync();
    //             return result;
    //         }
    //         catch (Exception e)
    //         {
    //             await _session.AbortTransactionAsync();
    //         }
    //     }
    //
    //     return result;
    // }

    public IContributionRepository ContributionRepository => new MongoContributionRepository(_session);

    public IEventOutboxRepository EventOutboxRepository => new MongoEventOutboxRepository(_session);
}