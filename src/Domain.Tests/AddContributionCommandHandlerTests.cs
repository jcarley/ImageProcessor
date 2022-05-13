using System;
using System.Threading;
using System.Threading.Tasks;

using Domain.Commands;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Tests.Support;

using FluentAssertions;

using Moq;

using Xunit;

namespace Domain.Tests;

public class AddContributionCommandHandlerTests
{
    [Fact(DisplayName = "Should successfully add a new contribution and publish an event")]
    public async Task Should_Successfully_AddANewContributionAndPublishEvent()
    {
        // Arrange
        Mock<IUnitOfWork> mockUnitOfWork = new();
        Mock<IContributionRepository> mockContributionRepository = new();
        Mock<IEventOutboxRepository> mockEventOutboxRepository = new();

        mockUnitOfWork
            .Setup(uow => uow.ContributionRepository).Returns(mockContributionRepository.Object);
        mockUnitOfWork
            .Setup(uow => uow.EventOutboxRepository).Returns(mockEventOutboxRepository.Object);

        MockTransactionalUnitOfWork transactionalUnitOfWorkUnitOfWork = new(mockUnitOfWork);

        AddContributionCommandHandler handler = new(transactionalUnitOfWorkUnitOfWork);

        // Act
        AddContributionCommand command = new()
        {
            Name = "New Contribution",
            Size = 1000,
            ImageType = "application/jpeg",
            ThumbnailUrl = "https://s3.amazon.com/thumbnails/contribution.jpg",
            HiResUrl = "https://s3.amazon.com/images/contribution.jpg",
            SampleHiResUrl = "https://s3.amazon.com/images/watermark_contribution.jpg",
        };

        AddContributionResult result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().BeEmpty();
        result.Succeeded.Should().BeTrue();

        mockContributionRepository
            .Verify(ex => ex.Add(It.IsAny<Contribution>()), Times.Once());

        mockEventOutboxRepository
            .Verify(ex => ex.Add(It.IsAny<OutBoxEvent>()), Times.Once());
    }

    [Fact(DisplayName = "Should fail adding a new contribution and does not publish an event")]
    public async Task Should_Fail_AddANewContributionAndDoesNotPublishEvent()
    {
        // Arrange
        Mock<IUnitOfWork> mockUnitOfWork = new();
        Mock<IContributionRepository> mockContributionRepository = new();
        Mock<IEventOutboxRepository> mockEventOutboxRepository = new();

        mockContributionRepository
            .Setup(repo => repo.Add(It.IsAny<Contribution>()))
            .Throws<Exception>();

        mockUnitOfWork
            .Setup(uow => uow.ContributionRepository).Returns(mockContributionRepository.Object);

        mockUnitOfWork
            .Setup(uow => uow.EventOutboxRepository).Returns(mockEventOutboxRepository.Object);

        MockTransactionalUnitOfWork transactionalUnitOfWorkUnitOfWork = new(mockUnitOfWork);

        AddContributionCommandHandler handler = new(transactionalUnitOfWorkUnitOfWork);

        // Act
        AddContributionCommand command = new()
        {
            Name = "New Contribution",
            Size = 1000,
            ImageType = "application/jpeg",
            ThumbnailUrl = "https://s3.amazon.com/thumbnails/contribution.jpg",
            HiResUrl = "https://s3.amazon.com/images/contribution.jpg",
            SampleHiResUrl = "https://s3.amazon.com/images/watermark_contribution.jpg",
        };

        AddContributionResult result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().NotBeEmpty();
        result.Succeeded.Should().BeFalse();

        mockContributionRepository
            .Verify(ex => ex.Add(It.IsAny<Contribution>()), Times.Once());

        mockEventOutboxRepository
            .Verify(ex => ex.Add(It.IsAny<OutBoxEvent>()), Times.Never());

        transactionalUnitOfWorkUnitOfWork.TransactionAborted.Should().BeTrue();
    }
}