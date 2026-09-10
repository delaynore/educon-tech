using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using FileService.Domain;
using FileService.Domain.Shared;
using FileService.UseCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using SharedKernel;

namespace FileService.Infrastructure.Postgres.Repositories;

public sealed class MediaAssetRepository : IMediaAssetRepository
{
    private readonly ILogger<MediaAssetRepository> _logger;
    private readonly FileServiceDbContext _fileServiceDbContext;

    public MediaAssetRepository(ILogger<MediaAssetRepository> logger, FileServiceDbContext fileServiceDbContext)
    {
        _logger = logger;
        _fileServiceDbContext = fileServiceDbContext;
    }

    public async Task<Result<Guid, Error>> AddAsync(
        MediaAsset mediaAsset,
        CancellationToken cancellationToken = default)
    {
        _fileServiceDbContext.Add(mediaAsset);

        try
        {
            await _fileServiceDbContext.SaveChangesAsync(cancellationToken);

            return mediaAsset.Id;
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException pgException)
        {
            _logger.LogError(
                e,
                "Database update error while creating media asset with id - {MediaAssetId}",
                mediaAsset.Id);

            return FileErrors.DatabaseError();
        }
        catch (OperationCanceledException e)
        {
            _logger.LogError(
                e,
                "Operation was cancelled exception while creating media asset with id - {MediaAssetId}",
                mediaAsset.Id);

            return FileErrors.OperationCancelled();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error while creating media asset with id - {MediaAssetId}", mediaAsset.Id);

            return FileErrors.DatabaseError();
        }
    }

    public async Task<Result<MediaAsset, Error>> GetByAsync(
        Expression<Func<MediaAsset, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var mediaAsset = await _fileServiceDbContext.MediaAssets.FirstOrDefaultAsync(predicate, cancellationToken);

        if (mediaAsset is null)
        {
            return GeneralErrors.NotFound(id: null, name: "mediaAsset");
        }

        return mediaAsset;
    }
}
