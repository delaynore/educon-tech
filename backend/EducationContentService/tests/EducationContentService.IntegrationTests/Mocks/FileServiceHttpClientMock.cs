using CSharpFunctionalExtensions;
using FileService.Contracts;
using FileService.Contracts.Dto;
using SharedKernel;

namespace EducationContentService.IntegrationTests.Mocks;

public class FileServiceHttpClientMock : IFileServiceHttpClient
{
    public Task<Result<GetMediaAssetsResponse, Error>> GetMediaAssets(GetMediaAssetsRequest request, CancellationToken cancellationToken)
    {
        var results = new GetMediaAssetsResponse([
            .. request.MediaAssetIds
                .Select(id => new GetMediaAssetsDto(id, "ready", "video", $"url/{id}")),
        ]);

        return Task.FromResult(Result.Success<GetMediaAssetsResponse, Error>(results));
    }
}
