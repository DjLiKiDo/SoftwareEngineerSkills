namespace SoftwareEngineerSkills.Domain.Exceptions;

/// <summary>
/// Exception thrown when a domain rule related to Dummy entities is violated
/// </summary>
public class DummyDomainException : DomainException
{
    /// <summary>
    /// Creates a new instance of the DummyDomainException class
    /// </summary>
    public DummyDomainException() : base()
    {
    }
    
    /// <summary>
    /// Creates a new instance of the DummyDomainException class with a specified error message
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception</param>
    public DummyDomainException(string message) : base(message)
    {
    }
    
    /// <summary>
    /// Creates a new instance of the DummyDomainException class with a specified error message and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public DummyDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}