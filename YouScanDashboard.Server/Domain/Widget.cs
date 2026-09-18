namespace YouScanDashboard.Server.Domain;

/// <summary>
/// A dashboard widget. Chart widgets show a <see cref="Dataset"/>; text widgets hold <see cref="Content"/>.
/// </summary>
public sealed class Widget
{
    // Required by EF Core for materialization.
    private Widget()
    {
    }

    private Widget(WidgetType type, int position, Dataset? dataset, string? content)
    {
        Id = Guid.NewGuid();
        Type = type;
        Position = position;
        Dataset = dataset;
        DatasetId = dataset?.Id;
        Content = content;
    }

    public Guid Id { get; private set; }

    public WidgetType Type { get; private set; }

    /// <summary>Order in the dashboard grid: a new widget is added to the end.</summary>
    public int Position { get; private set; }

    public Guid? DatasetId { get; private set; }

    public Dataset? Dataset { get; private set; }

    public string? Content { get; private set; }

    public static Widget CreateChart(WidgetType type, Dataset dataset, int position)
    {
        if (type == WidgetType.Text)
        {
            throw new ArgumentException("A text widget has no dataset: use CreateText.", nameof(type));
        }

        return new Widget(type, position, dataset, content: null);
    }

    public static Widget CreateText(int position) =>
        new(WidgetType.Text, position, dataset: null, content: string.Empty);

    /// <summary>Edit → Save flow of a text widget.</summary>
    public void UpdateContent(string content)
    {
        if (Type != WidgetType.Text)
        {
            throw new InvalidOperationException($"Widget '{Id}' of type '{Type}' has no text content.");
        }

        Content = content;
    }
}