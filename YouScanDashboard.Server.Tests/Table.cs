using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Tests;

/// <summary>Builds small tables for tests: <c>Of([Text("Product"), Number("Orders")], ["Laptops", "340"])</c>.</summary>
internal static class Table
{
    public static DatasetData Of(IReadOnlyList<DatasetColumn> columns, params string?[][] rows) => new(columns, rows);

    public static DatasetColumn Text(string name) => new(name, ColumnType.Text);

    public static DatasetColumn Number(string name) => new(name, ColumnType.Number);

    public static DatasetColumn Date(string name) => new(name, ColumnType.Date);
}
