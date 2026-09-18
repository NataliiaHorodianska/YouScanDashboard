using System.Globalization;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Charts;

/// <summary>Turns a stored table into chart-ready data.</summary>
public sealed class ChartDataBuilder(ChartColumnResolver columnResolver)
{
    public ChartData Build(DatasetData data)
    {
        if (columnResolver.Resolve(data) is not { } columns)
        {
            return new ChartData([], []);
        }

        var accumulator = new ChartAccumulator(SeriesNames(data, columns), data.Rows.Count);

        if (columns.SeriesIndex is { } seriesIndex)
        {
            AddLongTable(data, columns, seriesIndex, accumulator);
        }
        else
        {
            AddWideTable(data, columns, accumulator);
        }

        var chart = accumulator.Build();

        return columns.LabelType == ColumnType.Date ? chart with { Points = OrderByDate(chart.Points) } : chart;
    }

    /// <summary>Series of the chart: the value column names, or the values of the series column in a long table.</summary>
    private IReadOnlyList<string> SeriesNames(DatasetData data, ChartColumns columns)
    {
        if (columns.SeriesIndex is not { } seriesIndex)
        {
            var names = new string[columns.ValueIndexes.Count];
            for (var i = 0; i < names.Length; i++)
            {
                names[i] = data.Columns[columns.ValueIndexes[i]].Name;
            }

            return names;
        }

        var series = new List<string>();
        var known = new HashSet<string>(StringComparer.Ordinal);

        foreach (var row in data.Rows)
        {
            if (row[columns.LabelIndex] is not null && row[seriesIndex] is { } name && known.Add(name))
            {
                series.Add(name);
            }
        }

        return series;
    }

    /// <summary>Wide table: every value column is a series of its own.</summary>
    private void AddWideTable(DatasetData data, ChartColumns columns, ChartAccumulator accumulator)
    {
        foreach (var row in data.Rows)
        {
            if (row[columns.LabelIndex] is not { } label)
            {
                continue;
            }

            for (var i = 0; i < columns.ValueIndexes.Count; i++)
            {
                var valueIndex = columns.ValueIndexes[i];
                accumulator.Add(label, data.Columns[valueIndex].Name, ParseNumber(row[valueIndex]));
            }
        }
    }

    /// <summary>Long table: the Text column names the series, the last value column holds the value.</summary>
    private void AddLongTable(DatasetData data, ChartColumns columns, int seriesIndex, ChartAccumulator accumulator)
    {
        var valueIndex = columns.ValueIndexes[^1];

        foreach (var row in data.Rows)
        {
            if (row[columns.LabelIndex] is not { } label || row[seriesIndex] is not { } series)
            {
                continue;
            }

            accumulator.Add(label, series, ParseNumber(row[valueIndex]));
        }
    }

    // OrderBy reads the date of each label once, unlike a comparison that would parse it again on every compare.
    private IReadOnlyList<ChartPoint> OrderByDate(IReadOnlyList<ChartPoint> points) =>
        points.OrderBy(point => DateTime.Parse(point.Label, CultureInfo.InvariantCulture, DateTimeStyles.None)).ToList();

    private double? ParseNumber(string? text) =>
        text is null ? null : double.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
}