using YouScanDashboard.Server.Charts;
using YouScanDashboard.Server.Domain;
using static YouScanDashboard.Server.Tests.Table;

namespace YouScanDashboard.Server.Tests;

public sealed class ChartTypeSelectorTests
{
    private readonly ChartTypeSelector _selector = new(new ChartColumnResolver());

    [Fact]
    public void Date_label_is_a_line_chart()
    {
        var data = Of([Date("Date"), Number("Sales")],
            ["2025-01-01", "10"],
            ["2025-01-02", "12"]);

        Assert.Equal(WidgetType.LineChart, _selector.Select(data));
    }

    [Fact]
    public void Text_label_with_one_value_column_is_a_pie_chart()
    {
        var data = Of([Text("Product"), Number("Orders")],
            ["Laptops", "340"],
            ["Phones", "515"]);

        Assert.Equal(WidgetType.PieChart, _selector.Select(data));
    }

    [Fact]
    public void Text_label_with_several_value_columns_is_a_stacked_bar_chart()
    {
        var data = Of([Text("Region"), Number("Q1"), Number("Q2")],
            ["North", "10", "20"],
            ["South", "30", "40"]);

        Assert.Equal(WidgetType.StackedBarChart, _selector.Select(data));
    }

    [Fact]
    public void Negative_value_is_a_bar_chart_because_there_is_no_whole_to_split()
    {
        var data = Of([Text("Store"), Number("Result")],
            ["Kyiv", "-1250.5"],
            ["Lviv", "430"]);

        Assert.Equal(WidgetType.BarChart, _selector.Select(data));
    }

    [Fact]
    public void All_zero_values_are_a_bar_chart()
    {
        var data = Of([Text("Channel"), Number("Complaints")],
            ["Email", "0"],
            ["Phone", "0"]);

        Assert.Equal(WidgetType.BarChart, _selector.Select(data));
    }

    [Fact]
    public void Some_zero_values_still_make_a_pie_chart()
    {
        var data = Of([Text("Product"), Number("Orders")],
            ["Laptops", "340"],
            ["Tablets", "0"]);

        Assert.Equal(WidgetType.PieChart, _selector.Select(data));
    }

    [Fact]
    public void Numeric_labels_are_a_bar_chart()
    {
        var data = Of([Number("Year"), Number("Sales")],
            ["2023", "100"],
            ["2024", "150"]);

        Assert.Equal(WidgetType.BarChart, _selector.Select(data));
    }

    [Fact]
    public void Table_without_value_columns_has_no_chart()
    {
        var data = Of([Text("Name"), Text("City")],
            ["Anna", "Kyiv"]);

        Assert.Null(_selector.Select(data));
    }
}
