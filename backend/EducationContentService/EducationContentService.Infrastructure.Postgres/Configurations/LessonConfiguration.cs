using EducationContentService.Domain.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationContentService.Infrastructure.Postgres.Configurations;

public sealed class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("lessons");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");

        builder.OwnsOne(x => x.Title, navigationBuilder =>
        {
            navigationBuilder.Property(x => x.Value)
                .HasColumnName("title");

            navigationBuilder.HasIndex(x => x.Value)
                .IsUnique()
                .HasDatabaseName(DbIndex.LessonTitle);
        });

        builder.OwnsOne(x => x.Description, navigationBuilder =>
        {
            navigationBuilder.Property(x => x.Value)
                .HasColumnName("description");
        });

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false)
            .HasColumnName("is_deleted");

        builder.Property(x => x.DeletedAtUtc)
            .IsRequired(false)
            .HasColumnName("deleted_at");

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())")
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())")
            .HasColumnName("updated_at");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
