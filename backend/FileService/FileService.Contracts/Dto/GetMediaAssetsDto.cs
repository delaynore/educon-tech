namespace FileService.Contracts.Dto;

public sealed record GetMediaAssetsDto(Guid MediaAssetId, string Status, string AssetType, string? Url);
