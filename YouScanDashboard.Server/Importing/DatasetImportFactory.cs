using YouScanDashboard.Server.Charts;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Importing;

/// <summary>
/// Makes chart widgets out of tables read from a file.
/// For every table it picks the chart that fits (line, pie, bars) and creates a widget that shows it.
/// A table that fits no chart (e.g. only text, no numbers) is skipped.
/// Both the startup import from the SeedData folder and the "Upload file" button go through here,
/// so a file turns into the same widgets however it got into the system.
/// </summary>
public sealed class DatasetImportFactory(ChartTypeSelector chartTypeSelector)
{
    /// <summary>
    /// The name a table is remembered by once it is imported: file name, "#", table name,
    /// e.g. "stacked-bar.csv#stacked-bar". The startup import uses it to never import a table twice.
    /// </summary>
    public string SourceKey(string fileName, ImportedTable table) => $"{fileName}#{table.Name}";

    /// <summary>
    /// One widget for every table that can be shown as a chart.
    /// The widgets get positions one after another, starting at <paramref name="nextPosition"/>,
    /// so they appear at the end of the dashboard.
    /// </summary>
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