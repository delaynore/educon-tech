using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using FileService.Domain;
using SharedKernel;

namespace FileService.UseCases;

public interface IMediaAssetRepository
{
    Task<Result<Guid, Error>> AddAsync(MediaAsset mediaAsset, CancellationToken cancellationToken = default);

    Task<Result<MediaAsset, Error>> GetByAsync(
        Expression<Func<MediaAsset, bool>> predicate,
        CancellationToken cancellationToken = default);
}
