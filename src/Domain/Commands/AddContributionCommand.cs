using System.ComponentModel.DataAnnotations;

using Domain.Entities;

using MediatR;

namespace Domain.Commands;

public class AddContributionResult : TaskResult<AddContributionResult, Contribution>
{
}

public class AddContributionCommand : IRequest<AddContributionResult>
{
    [Required]
    public string? Name { get; set; }

    public string? ImageType { get; set; }

    public Int64 Size { get; set; }

    public string? HiResUrl { get; set; }

    public string? SampleHiResUrl { get; set; }

    public string? ThumbnailUrl { get; set; }
}