using Domain.Entities;

using MediatR;

namespace Domain.Commands;

public class GetContributionResult : TaskResult<GetContributionResult, Contribution>
{
}

public class GetContributionQuery : IRequest<GetContributionResult>
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? ImageType { get; set; }

    public Int64 Size { get; set; }

    public string? HiResUrl { get; set; }

    public string? SampleHiResUrl { get; set; }

    public string? ThumbnailUrl { get; set; }

    public ExifData? Exif { get; set; }
}