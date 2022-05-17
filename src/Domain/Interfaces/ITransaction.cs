namespace Domain.Interfaces;

public interface ITransaction : IDisposable
{
    bool AutoCommit { get; set; }

    object? DbTransaction { get; set; }

    void Commit();

    Task CommitAsync(CancellationToken cancellationToken = default);

    void Rollback();

    Task RollbackAsync(CancellationToken cancellationToken = default);
}