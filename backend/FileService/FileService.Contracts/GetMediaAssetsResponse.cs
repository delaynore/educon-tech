namespace FileService.Contracts;

public sealed record GetMediaAssetsResponse(IReadOnlyList<GetMediaAssetsDto> MediaAssets);
