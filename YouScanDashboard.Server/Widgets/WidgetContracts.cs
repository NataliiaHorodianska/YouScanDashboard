using System.ComponentModel.DataAnnotations;
using YouScanDashboard.Server.Charts;
using YouScanDashboard.Server.Domain;

namespace YouScanDashboard.Server.Widgets;

/// <summary>Widget in the dashboard list, without data: each widget loads its own data separately.</summary>
public sealed record WidgetSummaryResponse(Guid Id, WidgetType Type);

/// <summary>Widget with its data: chart data for chart widgets, content for text widgets.</summary>
public sealed record WidgetDetailsResponse(Guid Id, WidgetType Type, string? Content, ChartData? Chart);

public sealed record CreateWidgetRequest(
    [Required, EnumDataType(typeof(WidgetType))] WidgetType? Type);

public sealed record UpdateContentRequest(
    [Required(AllowEmptyStrings = true)] string? Content);