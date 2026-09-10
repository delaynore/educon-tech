using FileService.UseCases.Database;

namespace FileService.Infrastructure.Postgres;

public sealed class TransactionManager : ITransactionManager
{
    private readonly FileServiceDbContext _context;

    public TransactionManager(FileServiceDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
