using System.Data;

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

        if (_unitOfWork is not IAmTransactional transactional)
        {
            return AddContributionResult.Failed("Non transactional unit of work");
        }

        using (IDbTransaction transaction = transactional.BeginTransaction(_publisher))
        {
            try
            {
                // Will need to determine the result and take appropriate action
                // This is internal domain data
                await _unitOfWork.ContributionRepository.Add(contribution);

                // The even we want to publish
                ContributionAdded contributionAdded = new()
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contribution.Id,
                    CorrelationId = Guid.NewGuid(),
                    MessageId = Guid.NewGuid(),
                };

                // Store the new event in an outbox record to be published later
                OutBoxEvent outboxEvent = new()
                {
                    Id = Guid.NewGuid(),
                    Retries = 0,
                    CreatedAt = DateTime.Now,
                    OutputStream = "Contributions",
                    SerializedValue = contributionAdded,
                };

                // Save the new outbox event to the database
                await _unitOfWork.EventOutboxRepository.Add(outboxEvent);

                return AddContributionResult.Success(contribution);
            }
            catch (Exception e)
            {
                transaction.Rollback();

                return AddContributionResult.Failed("Failed");
            }
        }
    }
}