namespace FileService.Infrastructure.S3;

public sealed class S3Options
{
    public const string SectionName = "S3Options";

    public string Endpoint { get; init; }

    public string AccessKey { get; init; }

    public string SecretKey { get; init; }

    public bool WithSsl { get; init; }

    public int DownloadUrlExpirationHours { get; init; } = 24;

    public int UploadUrlExpirationHours { get; init; } = 1;

    public int MaxConcurrentRequests { get; init; } = 20;

    public int RecommendedChunkSizeBytes { get; init; } = 100 * 1024 * 1024; // 100 MB

    public int MaxChunks { get; init; } = 100;

    public IReadOnlyList<string> RequiredBuckets { get; init; } = [];
}
