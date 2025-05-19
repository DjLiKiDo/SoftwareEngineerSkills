using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.ValueObjects;

namespace SoftwareEngineerSkills.Domain.Entities.Customer;

/// <summary>
/// Customer aggregate root entity
/// </summary>
public class Customer : AggregateRoot
{
    /// <summary>
    /// The customer's name
    /// </summary>
    public string Name { get; private set; } = null!;
    
    /// <summary>
    /// The customer's email address
    /// </summary>
    public Email Email { get; private set; } = null!;
    
    /// <summary>
    /// The customer's phone number
    /// </summary>
    public string? PhoneNumber { get; private set; }
    
    // Private constructor for EF Core
    private Customer() { }
    
    /// <summary>
    /// Creates a new customer
    /// </summary>
    /// <param name="name">The customer's name</param>
    /// <param name="email">The customer's email address</param>
    public Customer(string name, Email email)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        
        AddDomainEvent(new CustomerCreatedEvent(Id, name, email.Value));
        EnforceInvariants();
    }
    
    /// <summary>
    /// Updates the customer's name
    /// </summary>
    /// <param name="name">The new name</param>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
            
        var oldName = Name;
        Name = name;
        
        AddDomainEvent(new CustomerNameUpdatedEvent(Id, oldName, name));
        EnforceInvariants();
    }
    
    /// <summary>
    /// Updates the customer's email address
    /// </summary>
    /// <param name="email">The new email address</param>
    public void UpdateEmail(Email email)
    {
        var oldEmail = Email;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        
        AddDomainEvent(new CustomerEmailUpdatedEvent(Id, oldEmail.Value, email.Value));
        EnforceInvariants();
    }
    
    /// <summary>
    /// Updates the customer's phone number
    /// </summary>
    /// <param name="phoneNumber">The new phone number</param>
    public void UpdatePhoneNumber(string? phoneNumber)
    {
        var oldPhoneNumber = PhoneNumber;
        PhoneNumber = phoneNumber;
        
        AddDomainEvent(new CustomerPhoneUpdatedEvent(Id, oldPhoneNumber, phoneNumber));
        EnforceInvariants();
    }
    
    /// <summary>
    /// Validates that the Customer entity satisfies all invariants
    /// </summary>
    /// <returns>A collection of error messages if any invariants are violated</returns>
    protected override IEnumerable<string> CheckInvariants()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            yield return "Customer name cannot be empty";
        }
        
        if (Email == null)
        {
            yield return "Customer email cannot be null";
        }
    }
}

/// <summary>
/// Event raised when a customer is created
/// </summary>
public class CustomerCreatedEvent : DomainEvent
{
    /// <summary>
    /// The customer's ID
    /// </summary>
    public Guid CustomerId { get; }
    
    /// <summary>
    /// The customer's name
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// The customer's email address
    /// </summary>
    public string Email { get; }
    
    /// <summary>
    /// Creates a new CustomerCreatedEvent
    /// </summary>
    /// <param name="customerId">The customer's ID</param>
    /// <param name="name">The customer's name</param>
    /// <param name="email">The customer's email address</param>
    public CustomerCreatedEvent(Guid customerId, string name, string email)
    {
        CustomerId = customerId;
        Name = name;
        Email = email;
    }
}

/// <summary>
/// Event raised when a customer's name is updated
/// </summary>
public class CustomerNameUpdatedEvent : DomainEvent
{
    /// <summary>
    /// The customer's ID
    /// </summary>
    public Guid CustomerId { get; }
    
    /// <summary>
    /// The customer's old name
    /// </summary>
    public string OldName { get; }
    
    /// <summary>
    /// The customer's new name
    /// </summary>
    public string NewName { get; }
    
    /// <summary>
    /// Creates a new CustomerNameUpdatedEvent
    /// </summary>
    /// <param name="customerId">The customer's ID</param>
    /// <param name="oldName">The customer's old name</param>
    /// <param name="newName">The customer's new name</param>
    public CustomerNameUpdatedEvent(Guid customerId, string oldName, string newName)
    {
        CustomerId = customerId;
        OldName = oldName;
        NewName = newName;
    }
}

/// <summary>
/// Event raised when a customer's email is updated
/// </summary>
public class CustomerEmailUpdatedEvent : DomainEvent
{
    /// <summary>
    /// The customer's ID
    /// </summary>
    public Guid CustomerId { get; }
    
    /// <summary>
    /// The customer's old email address
    /// </summary>
    public string OldEmail { get; }
    
    /// <summary>
    /// The customer's new email address
    /// </summary>
    public string NewEmail { get; }
    
    /// <summary>
    /// Creates a new CustomerEmailUpdatedEvent
    /// </summary>
    /// <param name="customerId">The customer's ID</param>
    /// <param name="oldEmail">The customer's old email address</param>
    /// <param name="newEmail">The customer's new email address</param>
    public CustomerEmailUpdatedEvent(Guid customerId, string oldEmail, string newEmail)
    {
        CustomerId = customerId;
        OldEmail = oldEmail;
        NewEmail = newEmail;
    }
}

/// <summary>
/// Event raised when a customer's phone number is updated
/// </summary>
public class CustomerPhoneUpdatedEvent : DomainEvent
{
    /// <summary>
    /// The customer's ID
    /// </summary>
    public Guid CustomerId { get; }
    
    /// <summary>
    /// The customer's old phone number
    /// </summary>
    public string? OldPhoneNumber { get; }
    
    /// <summary>
    /// The customer's new phone number
    /// </summary>
    public string? NewPhoneNumber { get; }
    
    /// <summary>
    /// Creates a new CustomerPhoneUpdatedEvent
    /// </summary>
    /// <param name="customerId">The customer's ID</param>
    /// <param name="oldPhoneNumber">The customer's old phone number</param>
    /// <param name="newPhoneNumber">The customer's new phone number</param>
    public CustomerPhoneUpdatedEvent(Guid customerId, string? oldPhoneNumber, string? newPhoneNumber)
    {
        CustomerId = customerId;
        OldPhoneNumber = oldPhoneNumber;
        NewPhoneNumber = newPhoneNumber;
    }
}
