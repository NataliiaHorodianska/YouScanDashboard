namespace YouScanDashboard.Server.Charts;

/// <summary>
/// Collects chart points while a table is read. Labels keep the order in which they first appear,
/// and values that repeat for the same label and series are added up.
/// </summary>
internal sealed class ChartAccumulator(IReadOnlyList<string> series)
{
    private readonly List<string> _labels = [];
    private readonly Dictionary<string, Dictionary<string, double?>> _valuesByLabel = new(StringComparer.Ordinal);

    public void Add(string label, string seriesName, double? value)
    {
        if (!_valuesByLabel.TryGetValue(label, out var values))
        {
            // A new label starts with a gap in every series.
            values = series.ToDictionary(name => name, _ => (double?)null, StringComparer.Ordinal);
            _valuesByLabel.Add(label, values);
            _labels.Add(label);
        }

        values[seriesName] = Sum(values[seriesName], value);
    }

    public ChartData Build() =>
        new(series, _labels.Select(label => new ChartPoint(label, _valuesByLabel[label])).ToList());

    // A gap plus a gap stays a gap; otherwise a missing value counts as zero.
    private static double? Sum(double? current, double? value) =>
        current is null ? value : current + (value ?? 0);
}