using FileService.Domain;

namespace FileService.UseCases.Database;

public interface IFileReadDbContext
{
    IQueryable<MediaAsset> MediaAssetsQuery { get; }
}
