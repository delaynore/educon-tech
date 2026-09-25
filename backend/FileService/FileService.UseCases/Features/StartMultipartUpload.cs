using CSharpFunctionalExtensions;
using FileService.Contracts.Dto;
using FileService.Domain;
using FileService.UseCases.FilesStorage;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace FileService.UseCases.Features;

public sealed class StartMultipartUploadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/files/multipart-upload", async Task<EndpointResult<StartMultipartUploadResponse>>(
            [FromBody] StartMultipartUploadRequest request,
            [FromServices] StartMultipartUploadHandler handler,
            CancellationToken cancellationToken) => await handler.Handle(request, cancellationToken));
    }
}

public sealed class StartMultipartUploadHandler
{
    private readonly ILogger<StartMultipartUploadHandler> _logger;
    private readonly IS3Provider _s3Provider;
    private readonly IChunkSizeCalculator _chunkSizeCalculator;
    private readonly IMediaAssetRepository _mediaAssetRepository;

    public StartMultipartUploadHandler(
        ILogger<StartMultipartUploadHandler> logger,
        IS3Provider s3Provider,
        IChunkSizeCalculator chunkSizeCalculator,
        IMediaAssetRepository mediaAssetRepository)
    {
        _logger = logger;
        _s3Provider = s3Provider;
        _chunkSizeCalculator = chunkSizeCalculator;
        _mediaAssetRepository = mediaAssetRepository;
    }

    public async Task<Result<StartMultipartUploadResponse, Error>> Handle(
        StartMultipartUploadRequest request,
        CancellationToken cancellationToken)
    {
        var fileNameResult = FileName.Create(request.FileName);
        if (fileNameResult.IsFailure)
        {
            return fileNameResult.Error;
        }

        var contentTypeResult = ContentType.Create(request.ContentType);
        if (contentTypeResult.IsFailure)
        {
            return contentTypeResult.Error;
        }

        var chunkCalculationResult = _chunkSizeCalculator.Calculate(request.SizeBytes);
        if (chunkCalculationResult.IsFailure)
        {
            return chunkCalculationResult.Error;
        }

        var mediaDataResult = MediaData.Create(
            fileNameResult.Value,
            contentTypeResult.Value,
            request.SizeBytes,
            chunkCalculationResult.Value.TotalChunks);

        if (mediaDataResult.IsFailure)
        {
            return mediaDataResult.Error;
        }

        var mediaAssetResult = MediaAsset.CreateForUpload(mediaDataResult.Value, request.AssetType.ToAssetType());
        if (mediaAssetResult.IsFailure)
        {
            return mediaAssetResult.Error;
        }

        var dbResult = await _mediaAssetRepository.AddAsync(mediaAssetResult.Value, cancellationToken);
        if (dbResult.IsFailure)
        {
            return dbResult.Error;
        }

        var mediaAsset = mediaAssetResult.Value;
        var startUploadResult = await _s3Provider.StartMultipartUploadAsync(
            mediaAsset.Key,
            mediaAsset.MediaData,
            cancellationToken);
        if (startUploadResult.IsFailure)
        {
            return startUploadResult.Error;
        }

        var chunkUploadUrlsResult = await _s3Provider.GenerateAllChunksUploadUrlsAsync(
            mediaAsset.Key,
            startUploadResult.Value,
            chunkCalculationResult.Value.TotalChunks,
            cancellationToken);
        if (chunkUploadUrlsResult.IsFailure)
        {
            return chunkUploadUrlsResult.Error;
        }

        _logger.LogInformation(
            "Media asset started uploading: {MediaAssetId} with key: {StorageKey}",
            mediaAsset.Key,
            mediaAsset.Key);

        return new StartMultipartUploadResponse(
            mediaAsset.Id,
            startUploadResult.Value,
            chunkUploadUrlsResult.Value,
            chunkCalculationResult.Value.ChunkSizeBytes);
    }
}
