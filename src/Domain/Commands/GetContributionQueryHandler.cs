using Domain.Entities;
using Domain.Interfaces;

using MediatR;

namespace Domain.Commands;

public class GetContributionQueryHandler : IRequestHandler<GetContributionQuery, GetContributionResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetContributionQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetContributionResult> Handle(GetContributionQuery request, CancellationToken cancellationToken)
    {
        Contribution? contribution = await _unitOfWork.ContributionRepository.FindById(request.Id, cancellationToken);

        if (contribution == null)
        {
            return GetContributionResult.Failed($"Unable to find contribution with id {request.Id}");
        }

        return GetContributionResult.Success(contribution);
    }
}