using YouScanDashboard.Server.Charts;
using static YouScanDashboard.Server.Tests.Table;

namespace YouScanDashboard.Server.Tests;

public sealed class ChartDataBuilderTests
{
    private readonly ChartDataBuilder _builder = new(new ChartColumnResolver());

    [Fact]
    public void Wide_table_has_a_series_per_value_column()
    {
        var data = Of([Text("Region"), Number("Q1"), Number("Q2")],
            ["North", "10", "20"],
            ["South", "30", "40"]);

        var chart = _builder.Build(data);

        Assert.Equal(new[] { "Q1", "Q2" }, chart.Series);
        Assert.Equal(new[] { "North", "South" }, chart.Points.Select(point => point.Label));
        Assert.Equal(40d, chart.Points[1].Values["Q2"]);
    }

    [Fact]
    public void Repeated_label_is_summed_and_keeps_its_first_position()
    {
        var data = Of([Text("Product"), Number("Orders")],
            ["Laptops", "1"],
            ["Phones", "2"],
            ["Laptops", "3"]);

        var chart = _builder.Build(data);

        Assert.Equal(new[] { "Laptops", "Phones" }, chart.Points.Select(point => point.Label));
        Assert.Equal(4d, chart.Points[0].Values["Orders"]);
    }

    [Fact]
    public void Long_table_takes_series_names_from_the_text_column()
    {
        var data = Of([Date("Date"), Text("Channel"), Number("Mentions")],
            ["2025-01-01", "Twitter", "5"],
            ["2025-01-01", "News", "3"],
            ["2025-01-02", "Twitter", "7"]);

        var chart = _builder.Build(data);

        Assert.Equal(new[] { "Twitter", "News" }, chart.Series);
        Assert.Equal(2, chart.Points.Count);
        Assert.Equal(7d, chart.Points[1].Values["Twitter"]);
        // No row for News on the second day: a gap, not a zero.
        Assert.Null(chart.Points[1].Values["News"]);
    }

    [Fact]
    public void Date_labels_are_sorted_chronologically()
    {
        var data = Of([Date("Date"), Number("Sales")],
            ["2025-03-01", "3"],
            ["2025-01-15", "1"],
            ["2025-02-01", "2"]);

        var chart = _builder.Build(data);

        Assert.Equal(new[] { "2025-01-15", "2025-02-01", "2025-03-01" }, chart.Points.Select(point => point.Label));
    }

    [Fact]
    public void Row_without_a_label_is_skipped()
    {
        var data = Of([Text("Product"), Number("Orders")],
            ["Laptops", "1"],
            [null, "5"]);

        var chart = _builder.Build(data);

        Assert.Equal("Laptops", Assert.Single(chart.Points).Label);
    }

    [Fact]
    public void Empty_value_is_a_gap_not_a_zero()
    {
        var data = Of([Text("Product"), Number("Orders")],
            ["Laptops", null]);

        var chart = _builder.Build(data);

        Assert.Null(Assert.Single(chart.Points).Values["Orders"]);
    }

    [Fact]
    public void Table_without_a_chart_gives_empty_data()
    {
        var data = Of([Text("Name")],
            ["Anna"]);

        var chart = _builder.Build(data);

        Assert.Empty(chart.Series);
        Assert.Empty(chart.Points);
    }
}
