namespace YouScanDashboard.Server.Domain;

/// <summary>
/// A table of the data folder that has already been imported.
/// The record outlives the widget created for it, so a restart does not import the same table again.
/// </summary>
public sealed class ImportedSource
{
    // Required by EF Core for materialization.
    private ImportedSource()
    {
    }

    private ImportedSource(string key) => Key = key;

    /// <summary>!!! File and sheet of the table, e.g.
    /// "stacked-bar.csv#stacked-bar".</summary>
    public string Key { get; private set; } = null!;

    public static ImportedSource Of(string key) => new(key);
}
