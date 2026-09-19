using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Data.Configurations;

public sealed class ImportedSourceConfiguration : IEntityTypeConfiguration<ImportedSource>
{
    public void Configure(EntityTypeBuilder<ImportedSource> builder)
    {
        // The key identifies the table: importing it twice is exactly what this table prevents.
        builder.HasKey(source => source.Key);

        builder.Property(source => source.Key).HasMaxLength(512);
    }
}