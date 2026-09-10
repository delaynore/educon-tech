namespace FileService.Contracts;

public sealed record CompleteMultipartUploadRequest(
    Guid MediaAssetId,
    string UploadId,
    IReadOnlyList<PartETagDto> PartETags);
