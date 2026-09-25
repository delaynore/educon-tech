using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using FileService.Contracts.Dto;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace FileService.Contracts.HttpCommunication;

internal sealed class FileServiceHttpClient : IFileServiceHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FileServiceHttpClient> _logger;

    public FileServiceHttpClient(
        HttpClient httpClient,
        ILogger<FileServiceHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<GetMediaAssetsResponse, Error>> GetMediaAssets(
        GetMediaAssetsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var httpResponse = await _httpClient.PostAsJsonAsync(
                "files/batch",
                request,
                cancellationToken);

            return await httpResponse
                .HandleResponseAsync<GetMediaAssetsResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media assets for {MediaAssetIds}", request.MediaAssetIds);

            return Error.Failure(
                "http.internal-server-error",
                "Failed to request media assets");
        }
    }
}
