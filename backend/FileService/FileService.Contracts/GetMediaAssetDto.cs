namespace FileService.Contracts;

public sealed record GetMediaAssetDto(
    Guid MediaAssetId,
    string Status,
    string AssetType,
    string? Url,
    long SizeBytes,
    string FileName,
    string ContentType);
