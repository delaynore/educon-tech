using System.Text.Json;
using FileService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Infrastructure.Postgres.Configurations;

public sealed class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.ToTable("media_assets");
        builder.HasKey(x => x.Id);

        builder.HasDiscriminator<string>("asset_type")
            .HasValue<VideoAsset>("video")
            .HasValue<PreviewAsset>("preview");

        builder.OwnsOne(m => m.MediaData, mb =>
        {
            mb.ToJson("media_data");

            mb.OwnsOne(md => md.ContentType, cb =>
            {
                cb.Property(x => x.Category).HasJsonPropertyName("category");
                cb.Property(x => x.Value).HasJsonPropertyName("value");
            });

            mb.OwnsOne(md => md.FileName, fb =>
            {
                fb.Property(x => x.Extension).HasJsonPropertyName("extension");
                fb.Property(x => x.Name).HasJsonPropertyName("name");
            });

            mb.Property(md => md.SizeBytes).HasJsonPropertyName("size_bytes");
            mb.Property(md => md.ExpectedChunksCount).HasJsonPropertyName("expected_chunks_count");
        });

        builder.Property(m => m.Id).HasColumnName("id");
        builder.Property(m => m.Status).HasColumnName("status");
        builder.Property(m => m.AssetType).HasColumnName("asset_type");

        builder.OwnsOne(m => m.Owner, ob =>
        {
            ob.Property(o => o.Context).HasColumnName("context");
            ob.Property(o => o.EntityId).HasColumnName("entity_id");
        });

        builder.Property(m => m.CreatedAt).HasColumnName("created_at");
        builder.Property(m => m.UpdatedAt).HasColumnName("updated_at");

        builder.Property(m => m.Key)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<StorageKey>(v, JsonSerializerOptions.Default))
            .HasColumnName("key")
            .HasColumnType("jsonb");

        builder.HasIndex(x => new { x.Status, x.CreatedAt });
    }
}
