using Microsoft.EntityFrameworkCore;
using YouScanDashboard.Server.Data;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Importing;

/// <summary>
/// Imports every supported file from the data folder on startup.
/// Idempotent: an already imported table (same file and sheet) is skipped,
/// so deleted widgets do not come back after a restart.
/// A file that cannot be read or saved is logged and skipped; other files are still imported.
/// </summary>
public sealed class DatasetSeeder(
    AppDbContext db,
    IEnumerable<ITableFileReader> readers,
    DatasetImportFactory importFactory,
    IHostEnvironment environment,
    ILogger<DatasetSeeder> logger)
{
    /// <summary>Data folder relative to the application content root; copied on publish by the project file.</summary>
    private const string DataFolder = "SeedData";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var directory = Path.Combine(environment.ContentRootPath, DataFolder);
        if (!Directory.Exists(directory))
        {
            logger.LogWarning("Data folder '{Directory}' does not exist.", directory);
            return;
        }

        var importedKeys = (await db.Datasets
                .Where(d => d.SourceKey != null)
                .Select(d => d.SourceKey!)
                .ToListAsync(cancellationToken))
            .ToHashSet(StringComparer.Ordinal);

        var nextPosition = await db.NextWidgetPositionAsync(cancellationToken);

        foreach (var path in Directory.EnumerateFiles(directory).Order(StringComparer.OrdinalIgnoreCase))
        {
            var fileName = Path.GetFileName(path);
            var reader = readers.FirstOrDefault(r => r.CanRead(fileName));

            if (reader is null)
            {
                logger.LogWarning("File '{FileName}' is skipped: unsupported format.", fileName);
                continue;
            }

            var newTables = ReadFile(reader, path, fileName)
                .Where(table => !importedKeys.Contains(importFactory.SourceKey(fileName, table)))
                .ToList();

            var imports = importFactory.CreateFromDataFolder(newTables, fileName, nextPosition);

            if (imports.Count == 0)
            {
                continue;
            }

            var widgets = imports.Select(import => import.Widget).OfType<Widget>().ToList();
            db.Datasets.AddRange(imports.Select(import => import.Dataset));
            db.Widgets.AddRange(widgets);

            try
            {
                await db.SaveChangesAsync(cancellationToken);
                nextPosition += widgets.Count;
                logger.LogInformation("Imported '{FileName}': {Tables} table(s), {Widgets} widget(s).",
                    fileName, imports.Count, widgets.Count);
            }
            catch (DbUpdateException exception)
            {              
                logger.LogError(exception, "File '{FileName}' could not be saved.", fileName);
                db.ChangeTracker.Clear();
            }
        }
    }

    private IReadOnlyList<ImportedTable> ReadFile(ITableFileReader reader, string path, string fileName)
    {
        try
        {
            using var stream = File.OpenRead(path);
            return reader.Read(stream, fileName);
        }
        catch (Exception exception)
        {
            // A broken file must not stop the application or the import of other files.
            logger.LogError(exception, "File '{FileName}' could not be read.", fileName);
            return [];
        }
    }
}