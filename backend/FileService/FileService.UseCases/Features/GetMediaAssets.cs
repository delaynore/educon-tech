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
using Microsoft.Extensions.Caching.Hybrid;
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
    private readonly IS3Provider _fileStorageProvider;
    private readonly HybridCache _hybridCache;
    private readonly FileStorageOptions _fileStorageOptions;

    public GetMediaAssetsHandler(
        ILogger<GetMediaAssetsHandler> logger,
        IFileReadDbContext context,
        IS3Provider fileStorageProvider,
        HybridCache hybridCache,
        FileStorageOptions fileStorageOptions)
    {
        _logger = logger;
        _context = context;
        _fileStorageProvider = fileStorageProvider;
        _hybridCache = hybridCache;
        _fileStorageOptions = fileStorageOptions;
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
        var keys = readyMediaAssets.Select(x => x.Key).ToList();

        var mediaUrlDictResult = await TryGetPresignedUrlsFromCache(keys, cancellationToken);

        if (!mediaUrlDictResult.IsFailure)
        {
            return mediaUrlDictResult.Error;
        }

        var results = mediaAssets
            .Select(mediaAsset => new GetMediaAssetsDto(
                mediaAsset.Id,
                mediaAsset.Status.ToString().ToLowerInvariant(),
                mediaAsset.AssetType.ToString().ToLowerInvariant(),
                mediaUrlDictResult.Value.GetValueOrDefault(mediaAsset.Key)))
            .ToList();

        return new GetMediaAssetsResponse(results);
    }

    private async Task<Result<Dictionary<StorageKey, string>, Error>> TryGetPresignedUrlsFromCache(
        List<StorageKey> storageKeys,
        CancellationToken cancellationToken)
    {
        if (!storageKeys.Any())
        {
            return Result.Success<Dictionary<StorageKey, string>, Error>([]);
        }

        var hybridCacheOptions = new HybridCacheEntryOptions()
        {
            Expiration = TimeSpan.FromHours(_fileStorageOptions.DownloadUrlExpirationHours - 1),
            LocalCacheExpiration = TimeSpan.FromHours(1),
        };

        var cachedUrlsTasks = storageKeys.Select(async storageKey =>
        {
            var url = await _hybridCache.GetOrCreateAsync<string>(
                storageKey.Value,
                _ => ValueTask.FromResult(default(string) !),
                hybridCacheOptions,
                cancellationToken: cancellationToken);

            return (storageKey, url);
        });

        var cachedUrls = await Task.WhenAll(cachedUrlsTasks);

        Dictionary<StorageKey, string> resultDict = [];
        List<StorageKey> storageKeysToGenerate = [];

        foreach (var (storageKey, url) in cachedUrls)
        {
            if (!string.IsNullOrEmpty(url))
            {
                resultDict.Add(storageKey, url);

                continue;
            }

            storageKeysToGenerate.Add(storageKey);
        }

        if (!storageKeysToGenerate.Any())
        {
            return resultDict;
        }

        var mediaUrlsResult = await _fileStorageProvider.GenerateDownloadUrlsAsync(
            storageKeys, cancellationToken);

        if (mediaUrlsResult.IsFailure)
        {
            _logger.LogError(
                "Failed to generate download URLs for {StorageKeysCount} storage keys. Error: {Error}",
                storageKeys.Count,
                mediaUrlsResult.Error);

            return mediaUrlsResult.Error;
        }

        foreach (var mediaUrl in mediaUrlsResult.Value)
        {
            resultDict[mediaUrl.StorageKey] = mediaUrl.PresignedUrl;
        }

        var setTasks = mediaUrlsResult.Value.Select(async mediaUrl =>
        {
            await _hybridCache.SetAsync(
                mediaUrl.StorageKey.Value,
                mediaUrl.PresignedUrl,
                hybridCacheOptions,
                cancellationToken: cancellationToken);
        });

        await Task.WhenAll(setTasks);

        return resultDict;
    }
}
