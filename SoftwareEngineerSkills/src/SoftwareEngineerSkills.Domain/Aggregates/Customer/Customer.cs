using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.ValueObjects;

namespace SoftwareEngineerSkills.Domain.Aggregates.Customer;

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
    
    /// <summary>
    /// The customer's shipping address
    /// </summary>
    public Address? ShippingAddress { get; private set; }
    
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
        
        AddAndApplyEvent(new CustomerCreatedEvent(Id, name, email.Value));
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
        
        AddAndApplyEvent(new CustomerNameUpdatedEvent(Id, oldName, name));
    }
    
    /// <summary>
    /// Updates the customer's email address
    /// </summary>
    /// <param name="email">The new email address</param>
    public void UpdateEmail(Email email)
    {
        var oldEmail = Email;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        
        AddAndApplyEvent(new CustomerEmailUpdatedEvent(Id, oldEmail.Value, email.Value));
    }
    
    /// <summary>
    /// Updates the customer's phone number
    /// </summary>
    /// <param name="phoneNumber">The new phone number</param>
    public void UpdatePhoneNumber(string? phoneNumber)
    {
        var oldPhoneNumber = PhoneNumber;
        PhoneNumber = phoneNumber;
        
        AddAndApplyEvent(new CustomerPhoneUpdatedEvent(Id, oldPhoneNumber, phoneNumber));
    }
    
    /// <summary>
    /// Updates the customer's shipping address
    /// </summary>
    /// <param name="address">The new shipping address</param>
    public void UpdateShippingAddress(Address address)
    {
        var oldAddress = ShippingAddress;
        ShippingAddress = address ?? throw new ArgumentNullException(nameof(address));
        
        AddAndApplyEvent(new CustomerAddressUpdatedEvent(Id, oldAddress, address));
    }
    
    /// <summary>
    /// Clears the customer's shipping address
    /// </summary>
    public void ClearShippingAddress()
    {
        var oldAddress = ShippingAddress;
        if (oldAddress != null)
        {
            ShippingAddress = null;
            AddAndApplyEvent(new CustomerAddressUpdatedEvent(Id, oldAddress, null));
        }
    }
    
    /// <summary>
    /// Updates the state of this aggregate root with a domain event
    /// </summary>
    /// <param name="domainEvent">The domain event that modifies the aggregate state</param>
    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case CustomerCreatedEvent created:
                // Constructor already sets these properties
                break;
                
            case CustomerNameUpdatedEvent nameUpdated:
                // The UpdateName method already sets the properties 
                break;
                
            case CustomerEmailUpdatedEvent emailUpdated:
                // The UpdateEmail method already sets the properties
                break;
                
            case CustomerPhoneUpdatedEvent phoneUpdated:
                // The UpdatePhoneNumber method already sets the properties
                break;
                
            case CustomerAddressUpdatedEvent addressUpdated:
                // The UpdateShippingAddress method already sets the properties
                break;
        }
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
