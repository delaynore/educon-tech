using EducationContentService.UseCases.Database;

namespace EducationContentService.Infrastructure.Postgres;

public sealed class TransactionManager : ITransactionManager
{
    private readonly EducationDbContext _context;

    public TransactionManager(EducationDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
