namespace BookStore.Core.Extensions;


public static class PrimitiveExtensions
{
    public static string ToCurrency(this decimal value) => value.ToString("C");

    public static bool IsBlank(this string? value) => string.IsNullOrWhiteSpace(value);

    public static string? NormalizeOrNull(this string? value)
    {
        var trimmed = value?.Trim();
        return trimmed.IsBlank() ? null : trimmed;
    }

    public static decimal? ToDecimalOrNull(this string? value) =>
        decimal.TryParse(value, out var result) ? result : null;

    public static int? ToIntOrNull(this string? value) =>
        int.TryParse(value, out var result) ? result : null;
}
