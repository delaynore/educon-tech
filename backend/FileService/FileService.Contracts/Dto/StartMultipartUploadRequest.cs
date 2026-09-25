namespace FileService.Contracts.Dto;

public sealed record StartMultipartUploadRequest(
    string FileName,
    string AssetType,
    string ContentType,
    long SizeBytes,
    string? Context = null,
    Guid? ContextId = null);
