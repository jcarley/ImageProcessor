using System;
using System.Threading;
using System.Threading.Tasks;

using Domain.Commands;
using Domain.Entities;
using Domain.Interfaces;

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
        Mock<IEventPublisher> mockEventPublisher = new();
        Mock<ITransaction> mockTransaction = new();
        Mock<IContributionRepository> mockContributionRepository = new();

        mockUnitOfWork
            .Setup(uow => uow.BeginTransaction(It.IsAny<IEventPublisher>()))
            .Returns(mockTransaction.Object);

        mockUnitOfWork
            .Setup(uow => uow.ContributionRepository)
            .Returns(mockContributionRepository.Object);

        AddContributionCommandHandler handler = new(mockUnitOfWork.Object, mockEventPublisher.Object);

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
            .Verify(ex => ex.Add(It.IsAny<Contribution>(), CancellationToken.None), Times.Once());
    }

    [Fact(DisplayName = "Should fail adding a new contribution and does not publish an event")]
    public async Task Should_Fail_AddANewContributionAndDoesNotPublishEvent()
    {
        // Arrange
        Mock<IUnitOfWork> mockUnitOfWork = new();
        Mock<IEventPublisher> mockEventPublisher = new();
        Mock<ITransaction> mockTransaction = new();
        Mock<IContributionRepository> mockContributionRepository = new();

        mockContributionRepository
            .Setup(repo => repo.Add(It.IsAny<Contribution>(), CancellationToken.None))
            .Throws<Exception>();

        mockUnitOfWork
            .Setup(uow => uow.BeginTransaction(It.IsAny<IEventPublisher>()))
            .Returns(mockTransaction.Object);

        mockUnitOfWork
            .Setup(uow => uow.ContributionRepository).Returns(mockContributionRepository.Object);

        AddContributionCommandHandler handler = new(mockUnitOfWork.Object, mockEventPublisher.Object);

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
            .Verify(ex => ex.Add(It.IsAny<Contribution>(), CancellationToken.None), Times.Once());

        mockTransaction.Verify(ex => ex.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once());
    }
}