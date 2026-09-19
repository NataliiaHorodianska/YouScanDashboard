using System.Globalization;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Charts;

/// <summary>Turns a stored table into chart-ready data: the series and a point for every label.</summary>
public sealed class ChartDataBuilder(ChartColumnResolver columnResolver)
{
    public ChartData Build(DatasetData data)
    {
        if (columnResolver.Resolve(data) is not { } columns)
        {
            return new ChartData([], []);
        }

        var accumulator = new ChartAccumulator(SeriesNames(data, columns));

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

    /// <summary>
    /// The series of the chart. In a wide table they are the names of the value columns;
    /// in a long table they are the names found in its text column, in the order they first appear.
    /// </summary>
    private IReadOnlyList<string> SeriesNames(DatasetData data, ChartColumns columns)
    {
        if (columns.SeriesIndex is not { } seriesIndex)
        {
            return columns.ValueIndexes.Select(index => data.Columns[index].Name).ToList();
        }
        var series = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var row in data.Rows)
        {
            if (row[columns.LabelIndex] is not null && row[seriesIndex] is { } name && seen.Add(name))
            {
                series.Add(name);
            }
        }
        return series;
    }

    /// <summary>Wide table, eg Brand, Positive, Negative:
    /// every value column is a series of its own.
    /// </summary>
    private void AddWideTable(DatasetData data, ChartColumns columns, ChartAccumulator accumulator)
    {
        foreach (var row in data.Rows)
        {
            if (row[columns.LabelIndex] is not { } label)
            {
                continue;
            }

            foreach (var valueIndex in columns.ValueIndexes)
            {
                accumulator.Add(label, data.Columns[valueIndex].Name, ParseNumber(row[valueIndex]));
            }
        }
    }

    /// <summary>
    /// Long table, eg Date, Channel, Mentions: the text column names the series
    /// and the last number column holds the value.
    /// </summary>
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

    // Labels are dates written as text ("2024-10-28"), so they are turned back into dates to be sorted.
    private IReadOnlyList<ChartPoint> OrderByDate(IReadOnlyList<ChartPoint> points) =>
        points.OrderBy(point => DateTime.Parse(point.Label, CultureInfo.InvariantCulture)).ToList();

    // Only cells of a number column get here, and that column was recognised as numbers
    // because every value in it parsed, so Parse cannot fail.
    private double? ParseNumber(string? text) =>
        text is null ? null : double.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
}