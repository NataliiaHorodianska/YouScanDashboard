using YouScanDashboard.Server.Data;
using YouScanDashboard.Server.Domain;
using YouScanDashboard.Server.Widgets;

namespace YouScanDashboard.Server.Importing;

public enum UploadError
{
    UnsupportedFormat,
    UnreadableFile,
    NoChartTables
}

public sealed record UploadResult(IReadOnlyList<WidgetSummaryResponse> Widgets, UploadError? Error)
{
    public static UploadResult Succeeded(IReadOnlyList<WidgetSummaryResponse> widgets) => new(widgets, null);

    public static UploadResult Failed(UploadError error) => new([], error);
}

/// <summary>Imports an uploaded file with the same mechanism as the startup import.</summary>
public sealed class DataUploadService(
    AppDbContext db,
    IEnumerable<ITableFileReader> readers,
    DatasetImportFactory importFactory,
    ILogger<DataUploadService> logger)
{
    public async Task<UploadResult> UploadAsync(string fileName, Stream stream, CancellationToken cancellationToken)
    {
        var reader = readers.FirstOrDefault(r => r.CanRead(fileName));
        if (reader is null)
        {
            return UploadResult.Failed(UploadError.UnsupportedFormat);
        }

        IReadOnlyList<ImportedTable> tables;
        try
        {
            tables = reader.Read(stream, fileName);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Uploaded file '{FileName}' could not be read.", fileName);
            return UploadResult.Failed(UploadError.UnreadableFile);
        }

        var nextPosition = await db.NextWidgetPositionAsync(cancellationToken);
        var widgets = importFactory
            .CreateFromUpload(tables, nextPosition)
            .Select(import => import.Widget)
            .OfType<Widget>()
            .ToList();

        if (widgets.Count == 0)
        {
            return UploadResult.Failed(UploadError.NoChartTables);
        }

        db.Widgets.AddRange(widgets);
        await db.SaveChangesAsync(cancellationToken);

        return UploadResult.Succeeded(widgets.Select(w => new WidgetSummaryResponse(w.Id, w.Type)).ToList());
    }
}