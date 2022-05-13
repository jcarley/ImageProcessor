namespace Domain.Interfaces;

public interface IUnitOfWork
{
    public IContributionRepository ContributionRepository { get; }

    public IEventOutboxRepository EventOutboxRepository { get; }
}