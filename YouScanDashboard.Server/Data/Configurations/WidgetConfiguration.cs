using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Data.Configurations;

public sealed class WidgetConfiguration : IEntityTypeConfiguration<Widget>
{
    public void Configure(EntityTypeBuilder<Widget> builder)
    {
        builder.Property(w => w.Type).HasConversion<string>();

        // Used to sort the grid and to find the next position.
        builder.HasIndex(w => w.Position);

        // A widget cannot exist without its data:
        // deleting a dataset deletes its widget.
        builder.HasOne(w => w.Dataset)
            .WithMany()
            .HasForeignKey(w => w.DatasetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}