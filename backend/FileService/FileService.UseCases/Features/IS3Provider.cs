namespace FileService.UseCases.Features;

public interface IS3Provider
{
    Task UploadFileAsync(Stream stream, string bucketName, string key, string contentType, CancellationToken token);

    Task<string> GenerateDownloadUrlAsync(string bucketName, string key);
}
