using System.Net.Http.Json;
using Amazon.S3;
using FileService.Contracts.Dto;
using FileService.Domain;
using FileService.IntegrationTests.Infrastructure;
using FileService.UseCases.HttpCommunication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CompleteMultipartUploadRequest = FileService.Contracts.Dto.CompleteMultipartUploadRequest;
using CompleteMultipartUploadResponse = FileService.Contracts.Dto.CompleteMultipartUploadResponse;

namespace FileService.IntegrationTests.Features;

public sealed class MultipartUploadFileTests : FileServiceTestsBase
{
    private readonly IntegrationTestsWebFactory _webFactory;

    public MultipartUploadFileTests(IntegrationTestsWebFactory webFactory)
        : base(webFactory)
    {
        _webFactory = webFactory;
    }

    [Fact]
    public async Task MultipartUpload_FullCycle_PersistsMediaAsset()
    {
        var cancellationToken = CancellationToken.None;

        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources", "test_file_MP4_1280_10MG.mp4");
        var fileInfo = new FileInfo(filePath);

        var startResponse = await StartMultipartUpload(fileInfo, cancellationToken);

        await ExecuteInDbContext(cancellationToken, async (dbContext, _, ct) =>
        {
            var mediaAsset = await dbContext.MediaAssetsQuery
                .FirstOrDefaultAsync(x => x.Id == startResponse.MediaAssetId, ct);

            Assert.NotNull(mediaAsset);
            Assert.Equal(MediaStatus.Uploading, mediaAsset.Status);
            Assert.Equal(startResponse.MediaAssetId, mediaAsset.Id);
            Assert.Equal(startResponse.ChunkUploadUrls.Count, mediaAsset.MediaData.ExpectedChunksCount);
        });

        var parts = await UploadChunks(fileInfo, startResponse, cancellationToken);

        _ = await CompleteMultipartUpload(startResponse, parts, cancellationToken);

        await ExecuteInDbContext(cancellationToken, async (dbContext, services, ct) =>
        {
            var mediaAsset = await dbContext.MediaAssetsQuery
                .FirstOrDefaultAsync(x => x.Id == startResponse.MediaAssetId, ct);

            Assert.NotNull(mediaAsset);
            Assert.Equal(MediaStatus.Uploaded, mediaAsset.Status);

            var amazonS3Client = services.GetRequiredService<IAmazonS3>();
            var @object = await amazonS3Client.GetObjectAsync(
                mediaAsset.Key.Location,
                mediaAsset.Key.Value,
                ct);

            Assert.NotNull(@object);
            Assert.Equal(@object.ContentLength, mediaAsset.MediaData.SizeBytes);
            Assert.Equal(@object.Key, mediaAsset.Key.Value);
        });
    }

    private async Task<CompleteMultipartUploadResponse> CompleteMultipartUpload(
        StartMultipartUploadResponse startResponse,
        IReadOnlyList<PartETagDto> parts,
        CancellationToken cancellationToken)
    {
        var completeUploadRequest = new CompleteMultipartUploadRequest(
            startResponse.MediaAssetId,
            startResponse.UploadId,
            parts);

        var completeUploadResponse = await AppHttpClient.PostAsJsonAsync(
            "api/files/complete-upload",
            completeUploadRequest,
            cancellationToken);

        var responseResult = await completeUploadResponse
            .HandleResponseAsync<CompleteMultipartUploadResponse>(cancellationToken);

        Assert.True(responseResult.IsSuccess);
        Assert.NotNull(responseResult.Value.Key);

        return responseResult.Value;
    }

    private async Task<List<PartETagDto>> UploadChunks(
        FileInfo fileInfo,
        StartMultipartUploadResponse startResponse,
        CancellationToken cancellationToken)
    {
        await using var stream = fileInfo.OpenRead();

        var parts = new List<PartETagDto>();

        foreach (var chunkUploadUrl in startResponse.ChunkUploadUrls.OrderBy(x => x.PartNumber))
        {
            var chunk = new byte[startResponse.ChunkSizeBytes];
            var bytesRead = await stream.ReadAsync(chunk.AsMemory(0, chunk.Length), cancellationToken);

            if (bytesRead == 0)
            {
                break;
            }

            using var content = new ByteArrayContent(chunk, 0, bytesRead);

            var httpResponse = await HttpClient.PutAsync(
                chunkUploadUrl.UploadUrl,
                content,
                cancellationToken);

            var responseBode = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            var etag = httpResponse.Headers.ETag?.Tag.Trim('"');
            parts.Add(new PartETagDto(chunkUploadUrl.PartNumber, etag!));
        }

        return parts;
    }

    private async Task<StartMultipartUploadResponse> StartMultipartUpload(
        FileInfo fileInfo,
        CancellationToken cancellationToken)
    {
        var request = new StartMultipartUploadRequest(
            fileInfo.Name,
            "video",
            "video/mp4",
            fileInfo.Length);

        var response = await AppHttpClient.PostAsJsonAsync(
            "api/files/multipart-upload",
            request,
            cancellationToken);

        var responseResult = await response.HandleResponseAsync<StartMultipartUploadResponse>(cancellationToken);
        Assert.True(responseResult.IsSuccess);
        Assert.NotNull(responseResult.Value.UploadId);

        return responseResult.Value;
    }
}
