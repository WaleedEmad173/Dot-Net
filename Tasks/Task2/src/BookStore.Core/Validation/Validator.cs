using System.Text.RegularExpressions;

namespace BookStore.Core.Validation;

public static partial class Validator
{
    public static void RequireNonEmpty(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException($"{fieldName} cannot be empty.");
    }

    public static void RequirePositive(decimal value, string fieldName)
    {
        if (value <= 0)
            throw new ValidationException($"{fieldName} must be greater than zero.");
    }

    public static void RequireNonNegative(int value, string fieldName)
    {
        if (value < 0)
            throw new ValidationException($"{fieldName} cannot be negative.");
    }

    public static void RequirePositive(int value, string fieldName)
    {
        if (value <= 0)
            throw new ValidationException($"{fieldName} must be greater than zero.");
    }

    public static void RequireValidIsbn(string? isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn) || !IsbnRegex().IsMatch(isbn))
            throw new ValidationException("ISBN must be 10 or 13 digits (hyphens allowed).");
    }

    public static void RequireValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailRegex().IsMatch(email))
            throw new ValidationException("Email address is not valid.");
    }

    [GeneratedRegex(@"^[0-9]{9}[0-9Xx]$|^[0-9]{13}$|^(?:\d[- ]?){9}[\dXx]$|^(?:\d[- ]?){13}$")]
    private static partial Regex IsbnRegex();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
