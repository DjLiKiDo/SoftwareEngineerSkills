namespace SoftwareEngineerSkills.Domain.Common;

/// <summary>
/// Represents a domain error with a code and message
/// </summary>
public sealed class Error
{
    /// <summary>
    /// Gets the error code
    /// </summary>
    public string Code { get; }
    
    /// <summary>
    /// Gets the error message
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Creates a new instance of the Error class
    /// </summary>
    /// <param name="code">The error code</param>
    /// <param name="message">The error message</param>
    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    /// Creates an error indicating that an entity was not found
    /// </summary>
    /// <param name="entityName">The name of the entity</param>
    /// <returns>A new Error instance</returns>
    public static Error NotFound(string entityName)
        => new Error("NotFound", $"{entityName} was not found.");

    /// <summary>
    /// Creates an error indicating that an operation is not permitted
    /// </summary>
    /// <param name="reason">The reason why the operation is not permitted</param>
    /// <returns>A new Error instance</returns>
    public static Error Forbidden(string reason)
        => new Error("Forbidden", reason);

    /// <summary>
    /// Creates an error indicating that a validation rule was violated
    /// </summary>
    /// <param name="message">The validation error message</param>
    /// <returns>A new Error instance</returns>
    public static Error Validation(string message)
        => new Error("Validation", message);

    /// <summary>
    /// Creates an error indicating a conflict
    /// </summary>
    /// <param name="message">The conflict description</param>
    /// <returns>A new Error instance</returns>
    public static Error Conflict(string message)
        => new Error("Conflict", message);
}