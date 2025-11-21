using EducationContentService.Domain.Lessons;
using EducationContentService.Domain.ValueObjects;
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

        builder.Property(x => x.Title)
            .HasConversion(v => v.Value, v => Title.Create(v).Value)
            .HasColumnName("title");

        builder.Property(x => x.Description)
            .HasConversion(v => v.Value, v => Description.Create(v).Value)
            .HasColumnName("description");

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

        builder.HasIndex(x => x.Title)
            .IsUnique()
            .HasDatabaseName(DbIndex.LessonTitle);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
