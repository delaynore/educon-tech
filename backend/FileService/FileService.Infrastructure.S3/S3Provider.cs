using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using FileService.Contracts;
using FileService.Domain;
using FileService.UseCases.FilesStorage;
using FileService.UseCases.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel;
using CompleteMultipartUploadRequest = Amazon.S3.Model.CompleteMultipartUploadRequest;

namespace FileService.Infrastructure.S3;

public sealed class S3Provider : IDisposable, IS3Provider
{
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<S3Provider> _logger;
    private readonly FileStorageOptions _fileStorageOptions;

    private readonly SemaphoreSlim _maxConcurrentRequestsSemaphore;

    public S3Provider(
        IAmazonS3 s3Client,
        IOptions<FileStorageOptions> options,
        ILogger<S3Provider> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
        _fileStorageOptions = options.Value;
        _maxConcurrentRequestsSemaphore = new SemaphoreSlim(options.Value.MaxConcurrentRequests);
    }

    public async Task<Result<string, Error>> StartMultipartUploadAsync(
        StorageKey storageKey,
        MediaData mediaData,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new InitiateMultipartUploadRequest()
            {
                BucketName = storageKey.Location,
                Key = storageKey.Value,
                ContentType = mediaData.ContentType.Value,
            };

            var result = await _s3Client.InitiateMultipartUploadAsync(request, cancellationToken);

            return result.UploadId;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error starting multipart upload");

            return S3ErrorMapper.ToError(exception);
        }
    }

    public async Task<Result<IReadOnlyList<ChunkUploadUrl>, Error>> GenerateAllChunksUploadUrlsAsync(
        StorageKey storageKey,
        string uploadId,
        int totalChunks,
        CancellationToken cancellationToken)
    {
        try
        {
            var tasks = Enumerable.Range(1, totalChunks)
                .Select(async partNumber =>
                {
                    await _maxConcurrentRequestsSemaphore.WaitAsync(cancellationToken);
                    try
                    {
                        var request = new GetPreSignedUrlRequest()
                        {
                            BucketName = storageKey.Location,
                            Key = storageKey.Value,
                            Verb = HttpVerb.PUT,
                            UploadId = uploadId,
                            PartNumber = partNumber,
                            Expires = DateTime.UtcNow.AddHours(_fileStorageOptions.UploadUrlExpirationHours),
                            Protocol = _fileStorageOptions.WithSsl ? Protocol.HTTPS : Protocol.HTTP,
                        };

                        var url = await _s3Client.GetPreSignedURLAsync(request);

                        return new ChunkUploadUrl(partNumber, url);
                    }
                    finally
                    {
                        _maxConcurrentRequestsSemaphore.Release();
                    }
                });

            return await Task.WhenAll(tasks);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while generating upload urls");

            return S3ErrorMapper.ToError(e);
        }
    }

    public async Task<Result<string, Error>> CompleteMultipartUploadAsync(
        StorageKey storageKey,
        string uploadId,
        IReadOnlyList<PartETagDto> partETagsDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new CompleteMultipartUploadRequest()
            {
                BucketName = storageKey.Location,
                Key = storageKey.Value,
                UploadId = uploadId,
                PartETags = [.. partETagsDto.Select(x => new PartETag(x.PartNumber, x.ETag))],
            };

            var response = await _s3Client.CompleteMultipartUploadAsync(request, cancellationToken);

            return response.Key;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while complete multipart upload");

            return S3ErrorMapper.ToError(e);
        }
    }

    public async Task<Result<string, Error>> GenerateDownloadUrlAsync(StorageKey storageKey)
    {
        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = storageKey.Location,
                Key = storageKey.Value,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddHours(_fileStorageOptions.DownloadUrlExpirationHours),
                Protocol = _fileStorageOptions.WithSsl ? Protocol.HTTPS : Protocol.HTTP,
            };

            // todo: try catch for null string (file does not exist)
            return await _s3Client.GetPreSignedURLAsync(request);
        }
        catch (Exception e)
        {
            return S3ErrorMapper.ToError(e);
        }
    }

    public async Task<Result<IReadOnlyList<MediaUrl>, Error>> GenerateDownloadUrlsAsync(
        IEnumerable<StorageKey> storageKeys,
        CancellationToken cancellationToken)
    {
        try
        {
            var tasks = storageKeys
                .Select(async storageKey =>
                {
                    await _maxConcurrentRequestsSemaphore.WaitAsync(cancellationToken);
                    try
                    {
                        var request = new GetPreSignedUrlRequest
                        {
                            BucketName = storageKey.Location,
                            Key = storageKey.Value,
                            Verb = HttpVerb.GET,
                            Expires = DateTime.UtcNow.AddHours(_fileStorageOptions.DownloadUrlExpirationHours),
                            Protocol = _fileStorageOptions.WithSsl ? Protocol.HTTPS : Protocol.HTTP,
                        };

                        // todo: try catch for null string (file does not exist)
                        return new MediaUrl(storageKey, await _s3Client.GetPreSignedURLAsync(request));
                    }
                    finally
                    {
                        _maxConcurrentRequestsSemaphore.Release();
                    }
                });

            return await Task.WhenAll(tasks);
        }
        catch (Exception e)
        {
            return S3ErrorMapper.ToError(e);
        }
    }

    public void Dispose()
    {
        _maxConcurrentRequestsSemaphore.Release();
        _maxConcurrentRequestsSemaphore.Dispose();
    }
}
