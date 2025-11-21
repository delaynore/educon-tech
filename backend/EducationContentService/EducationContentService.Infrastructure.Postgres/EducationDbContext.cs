using EducationContentService.Domain.Lessons;
using EducationContentService.UseCases.Database;
using Microsoft.EntityFrameworkCore;

namespace EducationContentService.Infrastructure.Postgres;

public sealed class EducationDbContext : DbContext, IEducationReadDbContext
{
    public DbSet<Lesson> Lessons { get; set; } = null!;

    public IQueryable<Lesson> LessonQuery => Lessons.AsNoTracking().AsQueryable();

    // public DbSet<Module> Modules { get; set; } = null!;
    // public DbSet<ModuleItem> ModuleItems { get; set; } = null!;
    public EducationDbContext(DbContextOptions<EducationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EducationDbContext).Assembly);
    }
}
