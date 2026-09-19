using System.Globalization;

namespace YouScanDashboard.Server.Importing;

/// <summary>A non-empty cell as text, with the types its value can represent.</summary>
public readonly record struct FormattedCell(string Text, bool CanBeNumber, bool CanBeDate);

/// <summary>
/// Converts raw ExcelDataReader values (double, DateTime, string) to culture-independent text.
/// Text is recognized as a number only with a dot ("1234.5") and as a date only in ISO format ("2024-10-28"),
/// so ambiguous values like "4,20" or "01.10.2024" stay text instead of being guessed.
/// </summary>
public sealed class CellFormatter
{
    private static readonly string[] IsoDateFormats =
        ["yyyy-MM-dd", "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss"];

    /// <summary>Returns null for an empty cell.</summary>
    public FormattedCell? Format(object? value) => value switch
    {
        // A cell that is already a number still has to be finite: see FromText.
        double number => new FormattedCell(number.ToString(CultureInfo.InvariantCulture), double.IsFinite(number), CanBeDate: false),
        DateTime date => new FormattedCell(FormatDate(date), CanBeNumber: false, CanBeDate: true),
        string text when !string.IsNullOrWhiteSpace(text) => FromText(text.Trim()),
        null or string => null,
        _ => new FormattedCell(Convert.ToString(value, CultureInfo.InvariantCulture)!, CanBeNumber: false, CanBeDate: false),
    };

    private FormattedCell FromText(string text) => new(
        text,
        // NaN and Infinity are not chart values and cannot be written to JSON: such a cell stays text.
        CanBeNumber: double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) && double.IsFinite(number),
        CanBeDate: DateTime.TryParseExact(text, IsoDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _));

    private string FormatDate(DateTime date) =>
        date.ToString(date.TimeOfDay == TimeSpan.Zero ? "yyyy-MM-dd" : "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
}