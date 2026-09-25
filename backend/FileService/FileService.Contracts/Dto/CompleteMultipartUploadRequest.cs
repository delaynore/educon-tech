namespace FileService.Contracts.Dto;

public sealed record CompleteMultipartUploadRequest(
    Guid MediaAssetId,
    string UploadId,
    IReadOnlyList<PartETagDto> PartETags);
