using CSharpFunctionalExtensions;
using FileService.Contracts;
using FileService.UseCases.Database;
using FileService.UseCases.FilesStorage;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace FileService.UseCases.Features;

public sealed class CompleteMultipartUploadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/files/complete-upload", async Task<EndpointResult<CompleteMultipartUploadResponse>>(
            [FromBody] CompleteMultipartUploadRequest request,
            [FromServices] CompleteMultipartUploadHandler handler,
            CancellationToken cancellationToken) => await handler.Handle(request, cancellationToken));
    }
}

public sealed class CompleteMultipartUploadHandler
{
    private readonly ILogger<CompleteMultipartUploadHandler> _logger;
    private readonly IS3Provider _s3Provider;
    private readonly IMediaAssetRepository _mediaAssetRepository;
    private readonly ITransactionManager _transactionManager;

    public CompleteMultipartUploadHandler(
        ILogger<CompleteMultipartUploadHandler> logger,
        IS3Provider s3Provider,
        IMediaAssetRepository mediaAssetRepository,
        ITransactionManager transactionManager)
    {
        _logger = logger;
        _s3Provider = s3Provider;
        _mediaAssetRepository = mediaAssetRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<CompleteMultipartUploadResponse, Error>> Handle(
        CompleteMultipartUploadRequest request,
        CancellationToken cancellationToken)
    {
        var (_, isFailure, mediaAsset, error) =
            await _mediaAssetRepository.GetByAsync(x => x.Id == request.MediaAssetId, cancellationToken);
        if (isFailure)
        {
            return error;
        }

        if (mediaAsset.MediaData.ExpectedChunksCount != request.PartETags.Count)
        {
            return GeneralErrors.Failure("expected chunks count not equal actual");
        }

        var result = await _s3Provider.CompleteMultipartUploadAsync(
            mediaAsset.Key,
            request.UploadId,
            request.PartETags,
            cancellationToken);

        if (result.IsFailure)
        {
            mediaAsset.MarkFailed();
            await _transactionManager.SaveChangesAsync(cancellationToken);

            return result.Error;
        }

        mediaAsset.MarkUploaded();

        await _transactionManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("File uploaded successfully: {MediaAssetId}", mediaAsset.Id);

        return new CompleteMultipartUploadResponse(result.Value);
    }
}
