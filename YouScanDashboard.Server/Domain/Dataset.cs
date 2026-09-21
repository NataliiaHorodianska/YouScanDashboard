namespace YouScanDashboard.Server.Domain;

public sealed class Dataset
{
    // Required by EF Core for materialization.
    private Dataset()
    {
    }

    private Dataset(DatasetData data)
    {
        Id = Guid.NewGuid();
        Data = data;
    }

    public Guid Id { get; private set; }

    public DatasetData Data { get; private set; } = null!;

    public static Dataset Create(DatasetData data) => new(data);
}