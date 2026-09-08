using Amazon.S3;
using Amazon.S3.Model;
using FileService.UseCases.Features;
using Microsoft.Extensions.Options;

namespace FileService.Infrastructure.S3;

public sealed class S3Provider : IS3Provider
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Options _s3Options;

    public S3Provider(IAmazonS3 s3Client, IOptions<S3Options> options)
    {
        _s3Client = s3Client;
        _s3Options = options.Value;
    }

    public async Task UploadFileAsync(
        Stream stream,
        string bucketName,
        string key,
        string contentType,
        CancellationToken token)
    {
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = stream,
            ContentType = contentType,
        };

        await _s3Client.PutObjectAsync(request, token);
    }

    public async Task<string> GenerateDownloadUrlAsync(
        string bucketName,
        string key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddHours(_s3Options.DownloadUrlExpiresHours),
            Protocol = _s3Options.WithSsl ? Protocol.HTTPS : Protocol.HTTP,
        };

        // todo: trycatch for null string (file does not exist)
        return await _s3Client.GetPreSignedURLAsync(request);
    }

    public async Task<string> GenerateUploadUrlAsync(
        string bucketName,
        string key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddHours(_s3Options.DownloadUrlExpiresHours),
            Protocol = _s3Options.WithSsl ? Protocol.HTTPS : Protocol.HTTP,
        };

        // todo: trycatch for null string (file does not exist)
        return await _s3Client.GetPreSignedURLAsync(request);
    }
}
