using System.Globalization;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Charts;

/// <summary>Picks a chart type for a table from its column roles, not from column names.</summary>
public sealed class ChartTypeSelector(ChartColumnResolver columnResolver)
{
    /// <summary>Returns null when the table shape does not match any chart.</summary>
    public WidgetType? Select(DatasetData data)
    {
        if (columnResolver.Resolve(data) is not { } columns)
        {
            return null;
        }

        if (columns.LabelType == ColumnType.Date)
        {
            return WidgetType.LineChart;
        }

        // Pie slices and stacked segments are parts of a whole. There is no whole to split when the labels
        // are numbers (e.g. years), when a value is negative, or when every value is zero: such data is shown as bars.
        if (columns.LabelType == ColumnType.Number || !CanShowAsPartsOfWhole(data, columns.ValueIndexes))
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
            for (var i = 0; i < valueIndexes.Count; i++)
            {
                if (row[valueIndexes[i]] is not { } cell)
                {
                    continue;
                }

                var value = double.Parse(cell, NumberStyles.Float, CultureInfo.InvariantCulture);
                if (value < 0)
                {
                    return false;
                }

                hasPositive |= value > 0;
            }
        }

        return hasPositive;
    }
}
