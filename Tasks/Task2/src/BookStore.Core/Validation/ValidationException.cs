namespace BookStore.Core.Validation;

public sealed class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
