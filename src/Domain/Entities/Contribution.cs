using Domain.Interfaces;

namespace Domain.Entities;

public class Contribution : IIdentity
{
    public string? Name { get; set; }

    public string? ImageType { get; set; }

    public Int64 Size { get; set; }

    public string? HiResUrl { get; set; }

    public string? SampleHiResUrl { get; set; }

    public string? ThumbnailUrl { get; set; }

    public ExifData? Exif { get; set; }

    public Guid Id { get; set; }
}