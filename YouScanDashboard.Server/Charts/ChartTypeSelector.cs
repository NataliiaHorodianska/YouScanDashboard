using System.Globalization;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Charts;
/// <summary>Picks a chart type for a table from
/// its column roles, not from column names.
/// </summary>
public sealed class ChartTypeSelector(ChartColumnResolver columnResolver)
{
    /// <summary>Returns null when the table
    /// shape does not match any chart.
    /// </summary>
    public WidgetType? Select(DatasetData data)
    {
        if (columnResolver.Resolve(data) is not { } columns)
        {
            return null;
        }
        // Dates on the X axis are a time series.
        if (columns.LabelType == ColumnType.Date)
        {
            return WidgetType.LineChart;
        }
        // Numbers as labels (e.g. years) are an axis,
        // not categories to compare as parts of a whole.
        if (columns.LabelType == ColumnType.Number)
        {
            return WidgetType.BarChart;
        }
        // A pie or a stacked bar splits a whole into parts, which makes no sense for negative values or all zeros.
        if (!CanShowAsPartsOfWhole(data, columns.ValueIndexes))
        {
            return WidgetType.BarChart;
        }
        return columns.ValueIndexes.Count == 1 ? WidgetType.PieChart : WidgetType.StackedBarChart;
    }

    private bool CanShowAsPartsOfWhole(DatasetData data, IReadOnlyList<int> valueIndexes)
    {
        var hasPositive = false;

        foreach (var row in data.Rows)
        {
            foreach (var index in valueIndexes)
            {
                if (row[index] is not { } cell)
                {
                    continue;
                }
                // A number column only holds values that parse, so Parse cannot fail here.
                var value = double.Parse(cell, NumberStyles.Float, CultureInfo.InvariantCulture);
                if (value < 0)
                {
                    return false;
                }
                if (value > 0)
                {
                    hasPositive = true;
                }
            }
        }
        return hasPositive;
    }
}