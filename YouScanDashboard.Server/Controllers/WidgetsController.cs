using Microsoft.AspNetCore.Mvc;
using YouScanDashboard.Server.Widgets;

namespace YouScanDashboard.Server.Controllers;

[ApiController]
[Route("api/widgets")]
public sealed class WidgetsController(WidgetService widgets) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<WidgetSummaryResponse>> GetAll(CancellationToken cancellationToken) =>
        widgets.GetAllAsync(cancellationToken);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WidgetDetailsResponse>> Get(Guid id, CancellationToken cancellationToken)
    {
        var widget = await widgets.GetAsync(id, cancellationToken);
        if (widget is null)
        {
            return NotFound();
        }
        return widget;
    }

    [HttpPost]
    public async Task<ActionResult<WidgetDetailsResponse>> Create(CreateWidgetRequest request, CancellationToken cancellationToken)
    {
        var widget = await widgets.CreateAsync(request.Type!.Value, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = widget.Id }, widget);
    }

    [HttpPut("{id:guid}/content")]
    public async Task<IActionResult> UpdateContent(Guid id, UpdateContentRequest request, CancellationToken cancellationToken)
    {
        var result = await widgets.UpdateContentAsync(id, request.Content!, cancellationToken);

        return result switch
        {
            UpdateContentResult.Updated => NoContent(),
            UpdateContentResult.NotFound => NotFound(),
            _ => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Only text widgets have content."),
        };
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (!await widgets.DeleteAsync(id, cancellationToken))
        {
            return NotFound();
        }
        return NoContent();
    }
}