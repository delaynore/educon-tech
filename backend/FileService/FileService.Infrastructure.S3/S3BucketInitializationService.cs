using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileService.Infrastructure.S3;

public sealed class S3BucketInitializationService : BackgroundService
{
    private readonly ILogger<S3BucketInitializationService> _logger;
    private readonly S3Options _options;
    private readonly IAmazonS3 _s3Client;

    public S3BucketInitializationService(
        IOptions<S3Options> s3Options,
        IAmazonS3 s3Client,
        ILogger<S3BucketInitializationService> logger)
    {
        _options = s3Options.Value;
        _s3Client = s3Client;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("S3 bucket initialization service started");

            if (_options.RequiredBuckets.Count == 0)
            {
                _logger.LogInformation("S3 bucket initialization service required buckets");

                throw new ArgumentException("Missing required buckets");
            }

            _logger.LogInformation(
                "S3 bucket initialization service started. Required buckets: {Buckets}",
                string.Join(",", _options.RequiredBuckets));

            var tasks = _options.RequiredBuckets
                .Select(bucketName => InitializeBucketAsync(bucketName, stoppingToken))
                .ToArray();

            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("S3 bucket initialization service was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Critical error during S3 bucket initialization");
            throw;
        }
    }

    private async Task InitializeBucketAsync(string bucketName, CancellationToken cancellationToken)
    {
        try
        {
            var bucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);

            if (bucketExist)
            {
                _logger.LogInformation("Bucket {BucketName} already exists", bucketName);

                return;
            }

            _logger.LogInformation("Creating bucket: {BucketName}", bucketName);

            var putBucketRequest = new PutBucketRequest
            {
                BucketName = bucketName,
            };

            await _s3Client.PutBucketAsync(putBucketRequest, cancellationToken);

            var policy = $$"""
                            {
                                "Version": "2012-10-17",
                                "Statement": [
                                    {
                                    "Effect": "Allow",
                                    "Principal": {
                                        "AWS": ["*"]
                                    },
                                    "Action": ["s3:GetObject"],
                                    "Resource": ["arn:aws:s3:::{{bucketName}}/*"]
                                    }
                                ]
                            }
                           """;

            var putBucketPolicyRequest = new PutBucketPolicyRequest
            {
                BucketName = bucketName,
                Policy = policy,
            };

            await _s3Client.PutBucketPolicyAsync(putBucketPolicyRequest, cancellationToken);

            _logger.LogInformation("Bucket {BucketName} created successfully", bucketName);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to initialize bucket {BucketName}", bucketName);
            throw;
        }
    }
}
