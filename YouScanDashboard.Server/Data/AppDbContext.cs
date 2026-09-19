using Microsoft.EntityFrameworkCore;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Dataset> Datasets => Set<Dataset>();

    public DbSet<Widget> Widgets => Set<Widget>();

    /// <summary>Tables of the data folder that were 
    /// already imported only. Only for saving keys</summary>
    public DbSet<ImportedSource> ImportedSources => Set<ImportedSource>();

    /// <summary>The position right after the last widget, so a new widget goes to the end of the grid.</summary>
    public async Task<int> NextWidgetPositionAsync(CancellationToken cancellationToken)
    {
        var lastPosition = await Widgets.MaxAsync(w => (int?)w.Position, cancellationToken);
        return lastPosition is null ? 0 : lastPosition.Value + 1;
    }

     /// <summary>
     /// All settings of the model. Input point
     /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}