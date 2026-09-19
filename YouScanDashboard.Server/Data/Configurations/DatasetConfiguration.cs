using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Data.Configurations;

public sealed class DatasetConfiguration : IEntityTypeConfiguration<Dataset>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    public void Configure(EntityTypeBuilder<Dataset> builder)
    {
        builder.Property(d => d.Data)
            .HasColumnType("jsonb")
            .HasConversion(
                data => JsonSerializer.Serialize(data, JsonOptions),
                json => JsonSerializer.Deserialize<DatasetData>(json, JsonOptions)!,
                // Data is only created, never modified in place: comparing references is enough
                // and avoids serializing the whole table on every change detection.
                new ValueComparer<DatasetData>(
                    (left, right) => ReferenceEquals(left, right),
                    data => RuntimeHelpers.GetHashCode(data),
                    data => data));
    }
}