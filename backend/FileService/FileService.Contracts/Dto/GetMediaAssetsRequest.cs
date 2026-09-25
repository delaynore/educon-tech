namespace FileService.Contracts.Dto;

public sealed record GetMediaAssetsRequest(IReadOnlyList<Guid> MediaAssetIds);
