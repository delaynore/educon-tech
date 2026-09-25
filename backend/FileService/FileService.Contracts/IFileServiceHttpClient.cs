using CSharpFunctionalExtensions;
using FileService.Contracts.Dto;
using SharedKernel;

namespace FileService.Contracts;

public interface IFileServiceHttpClient
{
    Task<Result<GetMediaAssetsResponse, Error>> GetMediaAssets(
        GetMediaAssetsRequest request,
        CancellationToken cancellationToken);
}
