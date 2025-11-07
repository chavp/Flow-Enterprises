namespace FlowShops.Core.Exceptions;

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : Exception
{
    public IEnumerable<string>? Errors { get; }

    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, IEnumerable<string> errors) 
        : base(message)
    {
        Errors = errors;
    }

    public ValidationException(IEnumerable<string> errors) 
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
