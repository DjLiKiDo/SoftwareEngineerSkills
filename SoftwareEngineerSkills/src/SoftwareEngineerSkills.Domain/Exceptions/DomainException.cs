namespace SoftwareEngineerSkills.Domain.Exceptions;

/// <summary>
/// Base exception for domain-specific errors
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Creates a new instance of the DomainException class
    /// </summary>
    public DomainException() : base()
    {
    }
    
    /// <summary>
    /// Creates a new instance of the DomainException class with a specified error message
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception</param>
    public DomainException(string message) : base(message)
    {
    }
    
    /// <summary>
    /// Creates a new instance of the DomainException class with a specified error message and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
