using Microsoft.EntityFrameworkCore;
using YouScanDashboard.Server.Charts;
using YouScanDashboard.Server.Data;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Widgets;

public enum UpdateContentResult
{
    Updated,
    NotFound,
    NotTextWidget
}

public sealed class WidgetService(
    AppDbContext db,
    ChartDataBuilder chartDataBuilder,
    RandomDatasetGenerator randomDatasetGenerator)
{
    public async Task<IReadOnlyList<WidgetSummaryResponse>> GetAllAsync(CancellationToken cancellationToken) =>
        await db.Widgets
            .OrderBy(w => w.Position)
            .ThenBy(w => w.Id)
            .Select(w => new WidgetSummaryResponse(w.Id, w.Type))
            .ToListAsync(cancellationToken);

    public async Task<WidgetDetailsResponse?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var widget = await db.Widgets
            .AsNoTracking()
            .Include(w => w.Dataset)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        return widget is null ? null : ToDetails(widget);
    }

    public async Task<WidgetDetailsResponse> CreateAsync(WidgetType type, CancellationToken cancellationToken)
    {
        var position = await db.NextWidgetPositionAsync(cancellationToken);

        var widget = type == WidgetType.Text
            ? Widget.CreateText(position)
            : Widget.CreateChart(type, Dataset.ForWidget(randomDatasetGenerator.Generate(type)), position);
        db.Widgets.Add(widget);
        await db.SaveChangesAsync(cancellationToken);

        return ToDetails(widget);
    }

    public async Task<UpdateContentResult> UpdateContentAsync(Guid id, string content, CancellationToken cancellationToken)
    {
        var widget = await db.Widgets.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if (widget is null)
        {
            return UpdateContentResult.NotFound;
        }

        if (widget.Type != WidgetType.Text)
        {
            return UpdateContentResult.NotTextWidget;
        }

        widget.UpdateContent(content);
        await db.SaveChangesAsync(cancellationToken);

        return UpdateContentResult.Updated;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var ownedDatasetId = await db.Widgets
            .Where(w => w.Id == id && w.Dataset != null && w.Dataset.SourceKey == null)
            .Select(w => w.DatasetId)
            .FirstOrDefaultAsync(cancellationToken);

        var deletedRows = ownedDatasetId is { } datasetId
            ? await db.Datasets.Where(d => d.Id == datasetId).ExecuteDeleteAsync(cancellationToken) // the widget is deleted by cascade
            : await db.Widgets.Where(w => w.Id == id).ExecuteDeleteAsync(cancellationToken);

        return deletedRows > 0;
    }

    private WidgetDetailsResponse ToDetails(Widget widget) => new(
        widget.Id,
        widget.Type,
        widget.Content,
        widget.Dataset is null ? null : chartDataBuilder.Build(widget.Dataset.Data));
}