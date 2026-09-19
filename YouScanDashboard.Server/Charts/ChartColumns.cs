using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Charts;

/// <summary>The role of each column of a table in a chart.</summary>
/// <param name="LabelIndex">Column with the X-axis labels (pie slice names).</param>
/// <param name="LabelType">Type of the label column: a Date label means a line chart.</param>
/// <param name="SeriesIndex">Column with series names in a long table, null in a wide table.</param>
/// <param name="ValueIndexes">Number columns with the values, without the label column.</param>
public readonly record struct ChartColumns(
    int LabelIndex,
    ColumnType LabelType,
    int? SeriesIndex,
    IReadOnlyList<int> ValueIndexes);

/// <summary>
/// Reads the role of each column from the column types, so that the chart type and the chart data
/// are always chosen by the same rule.
/// The label is the first Date column, otherwise the first Text column, otherwise the first Number column
/// (a table of numbers only, e.g. Year and Sales). The remaining Number columns hold the values.
/// A Date label together with a Text column means a long table, where that Text column names the series.
/// </summary>
public sealed class ChartColumnResolver
{
    /// <summary>Returns null when the table cannot be shown as a chart: no labels or no values.</summary>
    public ChartColumns? Resolve(DatasetData data)
    {
        var labelIndex = FirstIndex(data, ColumnType.Date)
            ?? FirstIndex(data, ColumnType.Text)
            ?? FirstIndex(data, ColumnType.Number);

        if (labelIndex is null)
        {
            return null;
        }
        var valueIndexes = ValueIndexes(data, labelIndex.Value);
        if (valueIndexes.Length == 0)
        {
            return null;
        }
        var labelType = data.Columns[labelIndex.Value].Type;
        var seriesIndex = labelType == ColumnType.Date ? FirstIndex(data, ColumnType.Text) : null;

        return new ChartColumns(labelIndex.Value, labelType, seriesIndex, valueIndexes);
    }

    private int? FirstIndex(DatasetData data, ColumnType type)
    {
        for (var i = 0; i < data.Columns.Count; i++)
        {
            if (data.Columns[i].Type == type)
            {
                return i;
            }
        }
        return null;
    }

    // Counted first so that the array is allocated once, with the exact size.
    private int[] ValueIndexes(DatasetData data, int labelIndex) =>
     Enumerable.Range(0, data.Columns.Count)
         .Where(i => i != labelIndex && data.Columns[i].Type == ColumnType.Number)
         .ToArray();
}