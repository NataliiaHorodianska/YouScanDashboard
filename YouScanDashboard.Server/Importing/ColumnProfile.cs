using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Importing;

/// <summary>
/// Collects evidence about a column while its cells are read and derives the column type:
/// Number or Date only if every non-empty cell can be one; otherwise Text.
/// </summary>
public sealed class ColumnProfile
{
    private bool _allNumbers = true;
    private bool _allDates = true;

    public bool HasValues { get; private set; }

    /// <summary>Meaningful only when <see cref="HasValues"/> is true.</summary>
    public ColumnType Type => _allNumbers ? ColumnType.Number : _allDates ? ColumnType.Date : ColumnType.Text;

    public void Observe(FormattedCell cell)
    {
        HasValues = true;
        _allNumbers &= cell.CanBeNumber;
        _allDates &= cell.CanBeDate;
    }
}