using Microsoft.AspNetCore.Mvc;
using YouScanDashboard.Server.Importing;
using YouScanDashboard.Server.Widgets;

namespace YouScanDashboard.Server.Controllers;

[ApiController]
[Route("api/imports")]
public sealed class ImportsController(DataUploadService uploads) : ControllerBase
{
    // 2 MB is plenty: that's tens of thousands of CSV rows, far more than anyone can read on a chart.
    // The real reason for the limit is memory. The whole file is parsed in memory before it's saved,
    // so one big upload could run the free Render instance (512 MB) out of memory and take the app down for everyone.
    private const int MaxUploadBytes = 2 * 1024 * 1024;

    /// <summary>Imports an .xlsx, .xls or .csv file: 
    /// every table that matches a chart becomes a widget.
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    // Answers 413 before the body is buffered, so an oversized file is never read at all.
    [RequestSizeLimit(MaxUploadBytes)]
    public async Task<ActionResult<IReadOnlyList<WidgetSummaryResponse>>> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "The file is empty.");
        }

        await using var stream = file.OpenReadStream();
        var result = await uploads.UploadAsync(Path.GetFileName(file.FileName), stream, cancellationToken);

        return result.Error switch
        {
            null => StatusCode(StatusCodes.Status201Created, result.Widgets),
            UploadError.UnsupportedFormat => Problem(statusCode: StatusCodes.Status415UnsupportedMediaType, title: "Unsupported file format."),
            UploadError.UnreadableFile => Problem(statusCode: StatusCodes.Status400BadRequest, title: "The file could not be read."),
            _ => Problem(statusCode: StatusCodes.Status422UnprocessableEntity, title: "The file has no table that can be shown as a chart."),
        };
    }
}