namespace FileService.Contracts;

public sealed record StartMultipartUploadRequest(
    string FileName,
    string AssetType,
    string ContentType,
    long Size,
    string Context,
    Guid ContextId);
