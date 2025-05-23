namespace SoftwareEngineerSkills.Domain.Exceptions;

/// <summary>
/// Exception thrown when a domain entity's invariants are violated.
/// Used specifically for enforcing entity state validation rules and constraints.
/// </summary>
public class DomainValidationException : BusinessRuleException
{
    /// <summary>
    /// Gets the list of validation errors
    /// </summary>
    public IReadOnlyList<string> Errors { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainValidationException"/> class
    /// </summary>
    public DomainValidationException() : base("One or more domain validation errors occurred.")
    {
        Errors = Array.Empty<string>();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainValidationException"/> class with a specified error message
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DomainValidationException(string message) : base(message)
    {
        Errors = new[] { message };
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainValidationException"/> class with a list of validation errors
    /// </summary>
    /// <param name="errors">The list of validation errors</param>
    public DomainValidationException(IEnumerable<string> errors) 
        : base(CreateMessage(errors))
    {
        Errors = errors.ToList().AsReadOnly();
    }
      /// <summary>
    /// Creates a formatted message from the validation errors
    /// </summary>
    /// <param name="errors">The validation errors</param>
    /// <returns>A formatted error message</returns>
    private static string CreateMessage(IEnumerable<string> errors)
    {
        var errorList = errors.ToList();
        return errorList.Count switch
        {
            0 => "One or more domain validation errors occurred.",
            1 => "One or more domain validation errors occurred.",
            _ => "One or more domain validation errors occurred."
        };
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainValidationException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public DomainValidationException(string message, Exception innerException) 
        : base(message, innerException)
    {
        Errors = new[] { message };
    }
}
