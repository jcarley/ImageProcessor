using System;
using System.Threading;
using System.Threading.Tasks;

using Api.Contributions;

using Domain.Commands;
using Domain.Entities;

using FluentAssertions;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace Api.Tests;

public class ContributionControllerTests
{
    [Fact(DisplayName = "Should successfully add a new contribution")]
    public async Task Should_Successfully_AddANewContribution()
    {
        Mock<IMediator> mediatorMock = new();

        Contribution expectedContribution = new()
        {
            Id = Guid.NewGuid(),
            Name = "New Contribution",
            Size = 1000,
            ImageType = "application/jpeg",
            ThumbnailUrl = "https://s3.amazon.com/thumbnails/contribution.jpg",
            HiResUrl = "https://s3.amazon.com/images/contribution.jpg",
            SampleHiResUrl = "https://s3.amazon.com/images/watermark_contribution.jpg",
        };

        mediatorMock
            .Setup(mediator =>
                mediator.Send(It.IsAny<AddContributionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AddContributionResult.Success(expectedContribution));

        ContributionController controller = new(mediatorMock.Object);

        AddContributionCommand command = new()
        {
            Name = "New Contribution",
            Size = 1000,
            ImageType = "application/jpeg",
            ThumbnailUrl = "https://s3.amazon.com/thumbnails/contribution.jpg",
            HiResUrl = "https://s3.amazon.com/images/contribution.jpg",
            SampleHiResUrl = "https://s3.amazon.com/images/watermark_contribution.jpg",
        };

        ActionResult<GetContributionQuery> result = await controller.Post(command);
        GetContributionQuery? actualCommand = (result.Result as CreatedAtRouteResult)?.Value as GetContributionQuery;

        actualCommand?.Id.Should().Be(expectedContribution.Id);
        actualCommand?.Name.Should().Be(expectedContribution.Name);
        actualCommand?.Size.Should().Be(expectedContribution.Size);
        actualCommand?.ImageType.Should().Be(expectedContribution.ImageType);
        actualCommand?.ThumbnailUrl.Should().Be(expectedContribution.ThumbnailUrl);
        actualCommand?.HiResUrl.Should().Be(expectedContribution.HiResUrl);
        actualCommand?.SampleHiResUrl.Should().Be(expectedContribution.SampleHiResUrl);
    }

    [Fact(DisplayName = "Should fail on missing required Name attribute")]
    public async Task Should_Fail_OnMissingRequiredNameAttribute()
    {
        Mock<IMediator> mediatorMock = new();

        mediatorMock
            .Setup(mediator =>
                mediator.Send(It.IsAny<AddContributionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AddContributionResult.Success(new Contribution()));

        ContributionController controller = new(mediatorMock.Object);

        AddContributionCommand command = new()
        {
            Name = null,
            Size = 1000,
            ImageType = "application/jpeg",
            ThumbnailUrl = "https://s3.amazon.com/thumbnails/contribution.jpg",
            HiResUrl = "https://s3.amazon.com/images/contribution.jpg",
            SampleHiResUrl = "https://s3.amazon.com/images/watermark_contribution.jpg",
        };

        ActionResult<GetContributionQuery> result = await controller.Post(command);
        GetContributionQuery? actualCommand = (result.Result as CreatedAtRouteResult)?.Value as GetContributionQuery;
    }
}