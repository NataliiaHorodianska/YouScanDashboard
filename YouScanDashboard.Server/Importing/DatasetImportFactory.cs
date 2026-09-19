using YouScanDashboard.Server.Charts;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Importing;

/// <summary>
/// Turns tables read from a file into chart widgets with their data.
/// Shared by the startup import from the data folder and by file uploads.
/// </summary>
public sealed class DatasetImportFactory(ChartTypeSelector chartTypeSelector)
{
    /// <summary>Identifies a table inside a file, e.g. "stacked-bar.csv#stacked-bar".</summary>
    public string SourceKey(string fileName, ImportedTable table) => $"{fileName}#{table.Name}";

    /// <summary>A widget for every table that matches a chart; a table that matches none is skipped.</summary>
    public IReadOnlyList<Widget> CreateWidgets(IReadOnlyList<ImportedTable> tables, int nextPosition)
    {
        var widgets = new List<Widget>(tables.Count);

        foreach (var table in tables)
        {
            if (chartTypeSelector.Select(table.Data) is not { } chartType)
            {
                continue;
            }

            // The dataset is saved together with its widget through the navigation property.
            widgets.Add(Widget.CreateChart(chartType, Dataset.Create(table.Data), nextPosition++));
        }

        return widgets;
    }
}