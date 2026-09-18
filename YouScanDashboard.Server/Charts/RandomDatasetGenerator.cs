using System.Globalization;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Charts;

/// <summary>
/// Generates random tables for newly created chart widgets:
/// a Text category column and one or more Number series columns.
/// Tables have the same shape as imported ones, so <see cref="ChartDataBuilder"/> renders them the same way.
/// </summary>
public sealed class RandomDatasetGenerator(Random random)
{
    private const int CategoryCount = 5;
    private const int MultiSeriesCount = 3;
    private const int MinValue = 10;
    private const int MaxValue = 100;

    public DatasetData Generate(WidgetType type) => type switch
    {
        WidgetType.LineChart or WidgetType.StackedBarChart => Generate(MultiSeriesCount),
        WidgetType.BarChart or WidgetType.PieChart => Generate(seriesCount: 1),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Random data is generated for chart widgets only."),
    };

    private DatasetData Generate(int seriesCount)
    {
        var columns = new List<DatasetColumn>(seriesCount + 1) { new("Category", ColumnType.Text) };

        for (var s = 1; s <= seriesCount; s++)
        {
            columns.Add(new DatasetColumn($"Series {s}", ColumnType.Number));
        }

        var rows = new List<IReadOnlyList<string?>>(CategoryCount);
        for (var r = 1; r <= CategoryCount; r++)
        {
            var row = new string?[columns.Count];
            row[0] = $"Category {r}";

            for (var c = 1; c < row.Length; c++)
            {
                row[c] = random.Next(MinValue, MaxValue + 1).ToString(CultureInfo.InvariantCulture);
            }

            rows.Add(row);
        }

        return new DatasetData(columns, rows);
    }
}