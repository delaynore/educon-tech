namespace FileService.Infrastructure.S3;

public sealed class S3Options
{
    public const string SectionName = "S3Options";

    public string Endpoint { get; init; }

    public string AccessKey { get; init; }

    public string SecretKey { get; init; }

    public bool WithSsl { get; init; }

    public int DownloadUrlExpiresHours { get; init; }

    public IReadOnlyList<string> RequiredBuckets { get; init; } = [];
}
