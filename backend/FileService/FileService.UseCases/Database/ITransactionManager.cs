namespace FileService.UseCases.Database;

public interface ITransactionManager
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
