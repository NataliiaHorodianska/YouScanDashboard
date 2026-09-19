namespace YouScanDashboard.Server.Domain;

/// <summary>The five widget types 
/// required by the task. Stored in the 
/// database as a string.</summary>
public enum WidgetType
{
    LineChart,
    BarChart,
    StackedBarChart,
    PieChart,
    Text
}