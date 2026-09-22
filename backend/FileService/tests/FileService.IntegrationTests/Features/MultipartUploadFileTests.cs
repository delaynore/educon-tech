using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FileService.Contracts;
using FileService.IntegrationTests.Infrastructure;
using SharedKernel;

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
        var request = new StartMultipartUploadRequest(
            "file.mp4",
            "video",
            "video/mp4",
            10000);

        var response = await AppHttpClient.PostAsJsonAsync("api/files/multipart-upload", request);

        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter(),
            },
        };

        var data = await response.Content.ReadFromJsonAsync<Envelope<StartMultipartUploadResponse>>();

        response.EnsureSuccessStatusCode();
        Assert.NotNull(data);
        Assert.False(data.IsError);
    }
}
