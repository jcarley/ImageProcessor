using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;

using MediatR;

namespace Domain.Commands;

public class AddContributionCommandHandler : IRequestHandler<AddContributionCommand, AddContributionResult>
{
    private readonly IEventPublisher _publisher;
    private readonly IUnitOfWork _unitOfWork;

    public AddContributionCommandHandler(IUnitOfWork unitOfWork, IEventPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _publisher = publisher;
    }

    public async Task<AddContributionResult> Handle(AddContributionCommand request, CancellationToken cancellationToken)
    {
        // This can be setup to be created via AutoMapper
        Contribution contribution = new()
        {
            Name = request.Name,
            Size = request.Size,
            ImageType = request.ImageType,
            ThumbnailUrl = request.ThumbnailUrl,
            HiResUrl = request.HiResUrl,
            SampleHiResUrl = request.SampleHiResUrl,
        };

        // using (ITransaction transaction = _unitOfWork.BeginTransaction(_publisher))
        using (ITransaction transaction = _unitOfWork.BeginTransaction(_publisher))
        {
            try
            {
                // Will need to determine the result and take appropriate action
                // This is internal domain data
                await _unitOfWork.ContributionRepository.Add(contribution, cancellationToken);

                // The even we want to publish
                ContributionAdded contributionAdded = new()
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contribution.Id,
                    CorrelationId = Guid.NewGuid(),
                    MessageId = Guid.NewGuid(),
                };

                await _publisher.Publish("contributions", contributionAdded);

                return AddContributionResult.Success(contribution);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);

                return AddContributionResult.Failed("Failed");
            }
        }
    }
}