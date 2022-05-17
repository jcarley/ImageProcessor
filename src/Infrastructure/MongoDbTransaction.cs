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