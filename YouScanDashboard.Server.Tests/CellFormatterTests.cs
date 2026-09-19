using YouScanDashboard.Server.Importing;

namespace YouScanDashboard.Server.Tests;

public sealed class CellFormatterTests
{
    private readonly CellFormatter _formatter = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Empty_cell_is_null(string? value)
    {
        Assert.Null(_formatter.Format(value));
    }

    [Fact]
    public void Number_cell_is_written_with_the_invariant_culture()
    {
        var cell = _formatter.Format(1234.5)!.Value;

        Assert.Equal("1234.5", cell.Text);
        Assert.True(cell.CanBeNumber);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Non_finite_number_cell_is_not_a_number(double value)
    {
        Assert.False(_formatter.Format(value)!.Value.CanBeNumber);
    }

    [Theory]
    [InlineData("1234.5")]
    [InlineData("-42")]
    [InlineData("1e3")]
    public void Text_with_a_dot_decimal_is_a_number(string text)
    {
        Assert.True(_formatter.Format(text)!.Value.CanBeNumber);
    }

    [Theory]
    [InlineData("4,20")]
    [InlineData("1,234.5")]
    [InlineData("NaN")]
    [InlineData("Infinity")]
    public void Ambiguous_or_non_finite_text_is_not_a_number(string text)
    {
        Assert.False(_formatter.Format(text)!.Value.CanBeNumber);
    }

    [Theory]
    [InlineData("2024-10-28")]
    [InlineData("2024-10-28T14:30")]
    public void Iso_text_is_a_date(string text)
    {
        Assert.True(_formatter.Format(text)!.Value.CanBeDate);
    }

    [Theory]
    [InlineData("01.10.2024")]
    [InlineData("10/28/2024")]
    public void Non_iso_text_is_not_a_date(string text)
    {
        Assert.False(_formatter.Format(text)!.Value.CanBeDate);
    }

    [Fact]
    public void Date_cell_without_time_is_written_as_an_iso_date()
    {
        var cell = _formatter.Format(new DateTime(2024, 10, 28))!.Value;

        Assert.Equal("2024-10-28", cell.Text);
        Assert.True(cell.CanBeDate);
    }
}
