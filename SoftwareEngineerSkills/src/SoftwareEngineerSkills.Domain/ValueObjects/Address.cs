using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Domain.ValueObjects;

/// <summary>
/// ValueObject that represents a physical address
/// </summary>
public class Address : ValueObject
{
    /// <summary>
    /// Gets the street line 1
    /// </summary>
    public string Street1 { get; }
    
    /// <summary>
    /// Gets the street line 2 (optional)
    /// </summary>
    public string? Street2 { get; }
    
    /// <summary>
    /// Gets the city
    /// </summary>
    public string City { get; }
    
    /// <summary>
    /// Gets the state or province
    /// </summary>
    public string State { get; }
    
    /// <summary>
    /// Gets the postal code
    /// </summary>
    public string PostalCode { get; }
    
    /// <summary>
    /// Gets the country
    /// </summary>
    public string Country { get; }
    
    // Private constructor for EF Core
    private Address() 
    {
        Street1 = string.Empty;
        City = string.Empty;
        State = string.Empty;
        PostalCode = string.Empty;
        Country = string.Empty;
    }
    
    /// <summary>
    /// Creates a new instance of the Address value object
    /// </summary>
    /// <param name="street1">Street line 1</param>
    /// <param name="city">City</param>
    /// <param name="state">State or province</param>
    /// <param name="postalCode">Postal code</param>
    /// <param name="country">Country</param>
    /// <param name="street2">Optional street line 2</param>
    /// <exception cref="BusinessRuleException">Thrown when address data is invalid</exception>
    public Address(
        string street1,
        string city,
        string state,
        string postalCode,
        string country,
        string? street2 = null)
    {
        if (string.IsNullOrWhiteSpace(street1))
            throw new BusinessRuleException("Street cannot be empty");
            
        if (string.IsNullOrWhiteSpace(city))
            throw new BusinessRuleException("City cannot be empty");
            
        if (string.IsNullOrWhiteSpace(state))
            throw new BusinessRuleException("State cannot be empty");
            
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new BusinessRuleException("Postal code cannot be empty");
            
        if (string.IsNullOrWhiteSpace(country))
            throw new BusinessRuleException("Country cannot be empty");
            
        Street1 = street1;
        Street2 = street2;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }
    
    /// <summary>
    /// Gets the components of this value object that are used in equality comparisons
    /// </summary>
    /// <returns>The equality components of this value object</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street1;
        if (Street2 != null) yield return Street2;
        yield return City;
        yield return State;
        yield return PostalCode;
        yield return Country;
    }
    
    /// <summary>
    /// Returns a string that represents the current object
    /// </summary>
    /// <returns>A string that represents the current address</returns>
    public override string ToString()
    {
        var street = Street2 == null ? Street1 : $"{Street1}, {Street2}";
        return $"{street}, {City}, {State} {PostalCode}, {Country}";
    }
}
