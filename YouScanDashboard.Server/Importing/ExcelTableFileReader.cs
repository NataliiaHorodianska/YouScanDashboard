using ExcelDataReader;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Importing;

/// <summary>
/// Read table of a .xlsx, .xls or .csv file using ExcelDataReader.
/// </summary>
public sealed class ExcelTableFileReader(CellFormatter formatter) : ITableFileReader
{
    public bool CanRead(string fileName) =>
        Path.GetExtension(fileName).ToLowerInvariant() is ".xlsx" or ".xls" or ".csv";

    public IReadOnlyList<ImportedTable> Read(Stream stream, string fileName)
    {
        var isCsv = Path.GetExtension(fileName).Equals(".csv", StringComparison.OrdinalIgnoreCase);

        using var reader = isCsv
            ? ExcelReaderFactory.CreateCsvReader(stream)
            : ExcelReaderFactory.CreateReader(stream);

        var tables = new List<ImportedTable>(reader.ResultsCount);
        do
        {
            var name = isCsv ? Path.GetFileNameWithoutExtension(fileName) : reader.Name;
            tables.Add(new ImportedTable(name, ReadTable(reader)));
        }
        while (reader.NextResult());

        return tables;
    }

    private DatasetData ReadTable(IExcelDataReader reader)
    {
        if (!reader.Read())
        {
            return new DatasetData([], []);
        }

        var headers = ReadHeaders(reader);
        var profiles = headers.Select(_ => new ColumnProfile()).ToArray();
        var rows = ReadRows(reader, profiles);

        var ordinals = Enumerable.Range(0, profiles.Length).Where(i => profiles[i].HasValues).ToArray();
        var names = ColumnNames(headers, ordinals);

        var columns = ordinals
            .Select((ordinal, index) => new DatasetColumn(names[index], profiles[ordinal].Type))
            .ToList();

        return new DatasetData(columns, SelectColumns(rows, ordinals, headers.Length));
    }

    private string[] ReadHeaders(IExcelDataReader reader)
    {
        var headers = new string[reader.FieldCount];
        for (var i = 0; i < headers.Length; i++)
        {
            headers[i] = formatter.Format(reader.GetValue(i))?.Text ?? string.Empty;
        }

        return headers;
    }

    private List<string?[]> ReadRows(IExcelDataReader reader, ColumnProfile[] profiles)
    {
        var rows = new List<string?[]>();

        while (reader.Read())
        {
            var row = new string?[profiles.Length];
            var isEmpty = true;

            for (var i = 0; i < row.Length; i++)
            {
                if (formatter.Format(reader.GetValue(i)) is not { } cell)
                {
                    continue;
                }

                row[i] = cell.Text;
                profiles[i].Observe(cell);
                isEmpty = false;
            }

            if (!isEmpty)
            {
                rows.Add(row);
            }
        }

        return rows;
    }

    // Column names become the series names of a chart, and every series needs its own name:
    // two columns called "Value" would collide when the chart data is built.
    // So a repeated name gets a number ("Value", "Value (2)", "Value (3)"),
    // and a column without a header is named after its position ("Column5").
    // Names are compared ignoring case: "Sales" and "sales" would look like a mistake in the legend.
    private List<string> ColumnNames(string[] headers, int[] ordinals)
    {
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var names = new List<string>(ordinals.Length);

        foreach (var ordinal in ordinals)
        {
            var baseName = headers[ordinal].Length > 0 ? headers[ordinal] : $"Column{ordinal + 1}";
            var name = baseName;

            for (var suffix = 2; !used.Add(name); suffix++)
            {
                name = $"{baseName} ({suffix})";
            }

            names.Add(name);
        }

        return names;
    }

    // Row arrays are reused as-is unless empty columns have to be dropped.
    // ** Filtering empty data in columns
    private IReadOnlyList<IReadOnlyList<string?>> SelectColumns(List<string?[]> rows, int[] ordinals, int columnCount)
    {
        if (ordinals.Length == columnCount)
        {
            return rows;
        }

        return rows
            .Select(row => (IReadOnlyList<string?>)Array.ConvertAll(ordinals, ordinal => row[ordinal]))
            .ToList();
    }
}