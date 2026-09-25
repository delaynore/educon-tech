using CSharpFunctionalExtensions;
using FileService.Contracts;
using FileService.Contracts.Dto;
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

public sealed class GetMediaAssetInfoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/files/{mediaAssetId}", async Task<EndpointResult<GetMediaAssetDto>>(
            [FromRoute] Guid mediaAssetId,
            [FromServices] GetMediaAssetInfoHandler handler,
            CancellationToken cancellationToken) => await handler.Handle(mediaAssetId, cancellationToken));
    }
}

public sealed record GetMediaAssetInfoHandler
{
    private readonly IFileReadDbContext _context;
    private readonly ILogger<GetMediaAssetsHandler> _logger;
    private readonly IS3Provider _provider;

    public GetMediaAssetInfoHandler(
        ILogger<GetMediaAssetsHandler> logger,
        IFileReadDbContext context,
        IS3Provider provider)
    {
        _logger = logger;
        _context = context;
        _provider = provider;
    }

    public async Task<Result<GetMediaAssetDto, Error>> Handle(
        Guid mediaAssetId,
        CancellationToken cancellationToken)
    {
        var mediaAsset = await _context.MediaAssetsQuery
            .FirstOrDefaultAsync(x => x.Id == mediaAssetId, cancellationToken);

        if (mediaAsset is null)
        {
            return GeneralErrors.NotFound(mediaAssetId, "mediaAsset");
        }

        string? url = null;
        if (mediaAsset.Status == MediaStatus.Ready)
        {
            var (_, isFailure, value, error) = await _provider.GenerateDownloadUrlAsync(mediaAsset.Key);
            if (isFailure)
            {
                return error;
            }

            url = value;
        }

        return new GetMediaAssetDto(
            mediaAsset.Id,
            mediaAsset.Status.ToString().ToLowerInvariant(),
            mediaAsset.AssetType.ToString().ToLowerInvariant(),
            url,
            mediaAsset.MediaData.SizeBytes,
            mediaAsset.MediaData.FileName.Name,
            mediaAsset.MediaData.ContentType.Value);
    }
}
