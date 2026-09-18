namespace YouScanDashboard.Server.Charts;

/// <summary>Chart-ready data: X-axis labels with a value per series. A null value is a gap.</summary>
public sealed record ChartData(IReadOnlyList<string> Series, IReadOnlyList<ChartPoint> Points);

public sealed record ChartPoint(string Label, IReadOnlyDictionary<string, double?> Values);