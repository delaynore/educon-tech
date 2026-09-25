namespace FileService.Contracts.Dto;

public sealed record GetMediaAssetsResponse(IReadOnlyList<GetMediaAssetsDto> MediaAssets);
