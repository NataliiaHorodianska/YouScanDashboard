namespace YouScanDashboard.Server.Domain;

/// <summary>
/// A table parsed from a file (or generated), stored as-is.
/// Cells are strings; null means an empty cell. Number and Date cells are parseable
/// with the invariant culture. Every row has exactly Columns.Count cells.
/// </summary>
public sealed record DatasetData(
    IReadOnlyList<DatasetColumn> Columns,
    IReadOnlyList<IReadOnlyList<string?>> Rows);

public sealed record DatasetColumn(string Name, 
    ColumnType Type);