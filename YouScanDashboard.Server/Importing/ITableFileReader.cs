using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Importing;

/// <summary>A table read from a file: sheet name (or file name for single-table formats) and its data.</summary>
public sealed record ImportedTable(string Name, DatasetData Data);

/// <summary>
/// Reads tables from a file of a supported format.
/// A new file format is supported by adding another implementation.
/// </summary>
public interface ITableFileReader
{
    bool CanRead(string fileName);

    IReadOnlyList<ImportedTable> Read(Stream stream, string fileName);
}