using Microsoft.EntityFrameworkCore;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Dataset> Datasets => Set<Dataset>();

    public DbSet<Widget> Widgets => Set<Widget>();

    /// <summary>Position after the last widget, so a new widget is added to the end of the grid.</summary>
    public async Task<int> NextWidgetPositionAsync(CancellationToken cancellationToken) =>
        (await Widgets.MaxAsync(w => (int?)w.Position, cancellationToken) ?? -1) + 1;

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}