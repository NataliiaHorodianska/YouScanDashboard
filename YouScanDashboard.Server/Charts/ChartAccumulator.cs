namespace YouScanDashboard.Server.Charts;

/// <summary>
/// Collects chart points while a table is read: values of a repeated label and series are summed,
/// and labels keep the order in which they appear.
/// Values are kept in one array per label and become a dictionary only in <see cref="Build"/>.
/// </summary>
internal sealed class ChartAccumulator
{
    private readonly IReadOnlyList<string> _series;
    private readonly Dictionary<string, int> _seriesIndexes;
    private readonly List<string> _labels;
    private readonly Dictionary<string, double?[]> _valuesByLabel;

    public ChartAccumulator(IReadOnlyList<string> series, int labelCapacity)
    {
        _series = series;
        _seriesIndexes = new Dictionary<string, int>(series.Count, StringComparer.Ordinal);
        for (var i = 0; i < series.Count; i++)
        {
            _seriesIndexes[series[i]] = i;
        }

        _labels = new List<string>(labelCapacity);
        _valuesByLabel = new Dictionary<string, double?[]>(labelCapacity, StringComparer.Ordinal);
    }

    public void Add(string label, string series, double? value)
    {
        var values = Values(label);
        var index = _seriesIndexes[series];

        values[index] = Sum(values[index], value);
    }

    public ChartData Build()
    {
        var points = new List<ChartPoint>(_labels.Count);

        foreach (var label in _labels)
        {
            var values = _valuesByLabel[label];
            var pointValues = new Dictionary<string, double?>(_series.Count, StringComparer.Ordinal);

            for (var i = 0; i < _series.Count; i++)
            {
                pointValues.Add(_series[i], values[i]);
            }

            points.Add(new ChartPoint(label, pointValues));
        }

        return new ChartData(_series, points);
    }

    private double?[] Values(string label)
    {
        if (_valuesByLabel.TryGetValue(label, out var values))
        {
            return values;
        }

        values = new double?[_series.Count];
        _valuesByLabel.Add(label, values);
        _labels.Add(label);

        return values;
    }

    private double? Sum(double? current, double? value) =>
        current is null ? value : value is null ? current : current + value;
}
