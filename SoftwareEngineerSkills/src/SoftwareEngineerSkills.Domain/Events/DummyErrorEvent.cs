namespace SoftwareEngineerSkills.Domain.Events;

/// <summary>
/// Event raised when an error occurs related to a dummy entity
/// </summary>
public class DummyErrorEvent : IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier of the dummy entity related to the error
    /// </summary>
    public Guid? DummyId { get; }
    
    /// <summary>
    /// Gets the error message
    /// </summary>
    public string ErrorMessage { get; }
    
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    public DateTime OccurredOn { get; }
    
    /// <summary>
    /// Gets the operation that was being performed when the error occurred
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Creates a new instance of the DummyErrorEvent class
    /// </summary>
    /// <param name="errorMessage">The error message</param>
    /// <param name="operation">The operation that was being performed</param>
    /// <param name="dummyId">The ID of the dummy entity related to the error (optional)</param>
    public DummyErrorEvent(string errorMessage, string operation, Guid? dummyId = null)
    {
        ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        Operation = operation ?? throw new ArgumentNullException(nameof(operation));
        DummyId = dummyId;
        OccurredOn = DateTime.UtcNow;
    }
}