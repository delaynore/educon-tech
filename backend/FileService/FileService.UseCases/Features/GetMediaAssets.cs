using CSharpFunctionalExtensions;
using FileService.Contracts;
using FileService.Domain;
using FileService.UseCases.Database;
using FileService.UseCases.FilesStorage;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace FileService.UseCases.Features;

public sealed class GetMediaAssetsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/files/batch", async Task<EndpointResult<GetMediaAssetsResponse>>(
            [FromBody] GetMediaAssetsRequest request,
            [FromServices] GetMediaAssetsHandler handler,
            CancellationToken cancellationToken) => await handler.Handle(request, cancellationToken));
    }
}

public sealed record GetMediaAssetsHandler
{
    private readonly IFileReadDbContext _context;
    private readonly ILogger<GetMediaAssetsHandler> _logger;
    private readonly IS3Provider _provider;

    public GetMediaAssetsHandler(
        ILogger<GetMediaAssetsHandler> logger,
        IFileReadDbContext context,
        IS3Provider provider)
    {
        _logger = logger;
        _context = context;
        _provider = provider;
    }

    public async Task<Result<GetMediaAssetsResponse, Error>> Handle(
        GetMediaAssetsRequest request,
        CancellationToken cancellationToken)
    {
        if (request.MediaAssetIds.Count == 0)
        {
            return new GetMediaAssetsResponse([]);
        }

        var mediaAssetIds = request.MediaAssetIds;

        var mediaAssets = await _context.MediaAssetsQuery
            .Where(m => mediaAssetIds.Contains(m.Id) && m.Status != MediaStatus.Deleted)
            .ToListAsync(cancellationToken);

        var readyMediaAssets = mediaAssets
            .Where(m => m.Status == MediaStatus.Ready);

        var mediaUrlsResult = await _provider.GenerateDownloadUrlsAsync(
            readyMediaAssets.Select(x => x.Key),
            cancellationToken);
        if (mediaUrlsResult.IsFailure)
        {
            return mediaUrlsResult.Error;
        }

        var mediaUrlDict = mediaUrlsResult.Value
            .ToDictionary(k => k.StorageKey, v => v.PresignedUrl);

        var results = mediaAssets
            .Select(mediaAsset => new GetMediaAssetsDto(
                mediaAsset.Id,
                mediaAsset.Status.ToString().ToLowerInvariant(),
                mediaAsset.AssetType.ToString().ToLowerInvariant(),
                mediaUrlDict.GetValueOrDefault(mediaAsset.Key)))
            .ToList();

        return new GetMediaAssetsResponse(results);
    }
}
