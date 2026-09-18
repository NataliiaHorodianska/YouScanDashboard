namespace YouScanDashboard.Server.Domain;

/// <summary>
/// Table data shown by a chart widget.
/// A table imported from the data folder has a <see cref="SourceKey"/> and is kept after its widget is deleted,
/// so the file is not imported again on restart. Any other dataset belongs to its widget and is deleted with it.
/// </summary>
public sealed class Dataset
{
    // Required by EF Core for materialization.
    private Dataset()
    {
    }

    private Dataset(string? sourceKey, DatasetData data)
    {
        Id = Guid.NewGuid();
        SourceKey = sourceKey;
        Data = data;
    }

    public Guid Id { get; private set; }

    /// <summary>File and sheet of a table imported from the data folder, e.g. "stacked-bar.csv#stacked-bar".</summary>
    public string? SourceKey { get; private set; }

    public DatasetData Data { get; private set; } = null!;

    public static Dataset FromFile(string sourceKey, DatasetData data) => new(sourceKey, data);

    /// <summary>Uploaded or randomly generated data of a single widget.</summary>
    public static Dataset ForWidget(DatasetData data) => new(sourceKey: null, data);
}