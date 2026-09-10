namespace FileService.Contracts;

public sealed record GetMediaAssetsDto(Guid MediaAssetId, string Status, string AssetType, string? Url);
