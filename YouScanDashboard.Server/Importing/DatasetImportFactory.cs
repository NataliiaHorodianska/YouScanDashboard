using YouScanDashboard.Server.Charts;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Importing;

/// <summary>A stored table and the chart widget created for it (null when no chart matches the table).</summary>
public sealed record DatasetImport(Dataset Dataset, Widget? Widget);

/// <summary>
/// Turns tables read from a file into datasets and chart widgets.
/// Shared by the startup import from the data folder and by file uploads.
/// </summary>
public sealed class DatasetImportFactory(ChartTypeSelector chartTypeSelector)
{
    /// <summary>Identifies a table inside a file of the data folder, e.g. "stacked-bar.csv#stacked-bar".</summary>
    public string SourceKey(string fileName, ImportedTable table) => $"{fileName}#{table.Name}";

    /// <summary>Data of the folder is kept after its widget is deleted, so the file is not imported again.</summary>
    public IReadOnlyList<DatasetImport> CreateFromDataFolder(IReadOnlyList<ImportedTable> tables, string fileName, int nextPosition) =>
        Create(tables, table => Dataset.FromFile(SourceKey(fileName, table), table.Data), nextPosition);

    /// <summary>Uploaded data belongs to its widget and is deleted together with it.</summary>
    public IReadOnlyList<DatasetImport> CreateFromUpload(IReadOnlyList<ImportedTable> tables, int nextPosition) =>
        Create(tables, table => Dataset.ForWidget(table.Data), nextPosition);

    private IReadOnlyList<DatasetImport> Create(
        IReadOnlyList<ImportedTable> tables,
        Func<ImportedTable, Dataset> createDataset,
        int nextPosition)
    {
        var imports = new List<DatasetImport>(tables.Count);

        foreach (var table in tables)
        {
            if (table.Data.Columns.Count == 0)
            {
                continue;
            }

            var dataset = createDataset(table);
            var widget = chartTypeSelector.Select(table.Data) is { } chartType
                ? Widget.CreateChart(chartType, dataset, nextPosition++)
                : null;

            imports.Add(new DatasetImport(dataset, widget));
        }

        return imports;
    }
}