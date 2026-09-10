namespace FileService.Contracts;

public sealed record GetMediaAssetsRequest(IReadOnlyList<Guid> MediaAssetIds);
