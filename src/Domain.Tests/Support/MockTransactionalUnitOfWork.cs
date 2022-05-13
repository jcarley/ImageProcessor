using System;
using System.Threading.Tasks;

using Domain.Interfaces;

using Moq;

namespace Domain.Tests.Support;

public class MockTransactionalUnitOfWork : IUnitOfWork, IAmTransactional
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;

    public MockTransactionalUnitOfWork(Mock<IUnitOfWork> mockUnitOfWork)
    {
        _mockUnitOfWork = mockUnitOfWork;
    }

    public bool TransactionAborted { get; private set; }

    public Task<TResult> WithTransaction<TResult>(Func<IUnitOfWork, Task<TResult>> callback)
    {
        return callback(this);
    }

    public void AbortTransaction()
    {
        TransactionAborted = true;
    }

    public IContributionRepository ContributionRepository => _mockUnitOfWork.Object.ContributionRepository;

    public IEventOutboxRepository EventOutboxRepository => _mockUnitOfWork.Object.EventOutboxRepository;
}