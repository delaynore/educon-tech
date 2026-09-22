using FileService.Domain;
using FileService.Infrastructure.Postgres.Configurations;
using FileService.UseCases.Database;
using Microsoft.EntityFrameworkCore;

namespace FileService.Infrastructure.Postgres;

public class FileServiceDbContext : DbContext, IFileReadDbContext
{
    public DbSet<MediaAsset> MediaAssets { get; set; } = null!;

    public IQueryable<MediaAsset> MediaAssetsQuery => MediaAssets.AsNoTracking().AsQueryable();

    public FileServiceDbContext(DbContextOptions<FileServiceDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FileServiceDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }
}
