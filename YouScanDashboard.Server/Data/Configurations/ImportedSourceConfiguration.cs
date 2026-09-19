using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Data.Configurations;

public sealed class ImportedSourceConfiguration : IEntityTypeConfiguration<ImportedSource>
{
    public void Configure(EntityTypeBuilder<ImportedSource> builder)
    {
        // The key identifies the table:
        // importing it twice is exactly what this table prevents.
        builder.HasKey(source => source.Key);
        // The key is only a file name, "#" and a table name, so it is always short.
        // The limit is just an extra precaution!!! and nothing relies on it: file names are never longer
        // than 255 characters, so a plain text column would work exactly the same.
        builder.Property(source => source.Key).HasMaxLength(512);
    }
}