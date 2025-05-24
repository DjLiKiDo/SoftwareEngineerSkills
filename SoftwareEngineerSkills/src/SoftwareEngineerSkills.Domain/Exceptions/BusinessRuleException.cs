namespace SoftwareEngineerSkills.Domain.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated during a domain operation.
/// Used when an operation cannot be completed due to a violation of domain rules.
/// </summary>
public class BusinessRuleException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessRuleException"/> class
    /// </summary>
    public BusinessRuleException() : base() { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessRuleException"/> class with a specified error message
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public BusinessRuleException(string message) : base(message) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessRuleException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public BusinessRuleException(string message, Exception innerException) 
        : base(message, innerException) { }
}
