namespace FileService.Contracts;

public sealed record StartMultipartUploadRequest(
    string FileName,
    string AssetType,
    string ContentType,
    long SizeBytes,
    string? Context = null,
    Guid? ContextId = null);
