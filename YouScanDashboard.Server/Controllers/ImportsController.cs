using Microsoft.AspNetCore.Mvc;
using YouScanDashboard.Server.Importing;
using YouScanDashboard.Server.Widgets;

namespace YouScanDashboard.Server.Controllers;

[ApiController]
[Route("api/imports")]
public sealed class ImportsController(DataUploadService uploads) : ControllerBase
{
    /// <summary>Imports an .xlsx, .xls or .csv file: every table that matches a chart becomes a widget.</summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
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