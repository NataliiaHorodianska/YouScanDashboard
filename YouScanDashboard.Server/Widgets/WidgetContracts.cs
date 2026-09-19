using System.ComponentModel.DataAnnotations;
using YouScanDashboard.Server.Charts;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Widgets;


public sealed record WidgetSummaryResponse(Guid Id, WidgetType Type);

public sealed record WidgetDetailsResponse(Guid Id, WidgetType Type,
    string? Content, ChartData? Chart);

public sealed record CreateWidgetRequest(
    [Required, EnumDataType(typeof(WidgetType))] WidgetType? Type);

public sealed record UpdateContentRequest(
    [Required(AllowEmptyStrings = true)] string? Content);