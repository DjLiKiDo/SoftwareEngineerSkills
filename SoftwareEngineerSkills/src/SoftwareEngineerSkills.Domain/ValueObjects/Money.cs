using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Domain.ValueObjects;

/// <summary>
/// Represents an immutable monetary value with currency
/// </summary>
public record Money
{
    /// <summary>
    /// Gets the amount
    /// </summary>
    public decimal Amount { get; }
    
    /// <summary>
    /// Gets the currency code (ISO 4217)
    /// </summary>
    public string CurrencyCode { get; }
    
    /// <summary>
    /// Private constructor to ensure validation through factory method
    /// </summary>
    private Money(decimal amount, string currencyCode)
    {
        Amount = amount;
        CurrencyCode = currencyCode;
    }
    
    /// <summary>
    /// Creates a new Money instance with validation
    /// </summary>
    /// <param name="amount">The monetary amount</param>
    /// <param name="currencyCode">The ISO 4217 currency code</param>
    /// <returns>A new Money value object</returns>
    /// <exception cref="DomainException">Thrown when currency code is invalid</exception>
    public static Money Create(decimal amount, string currencyCode)
    {
        // Validate currency code (basic validation)
        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            throw new DomainException("Currency code cannot be empty");
        }
        
        if (currencyCode.Length != 3)
        {
            throw new DomainException("Currency code must be 3 characters (ISO 4217 format)");
        }
        
        // In a real application, we might validate against a list of valid currency codes
        
        return new Money(amount, currencyCode.ToUpperInvariant());
    }
    
    /// <summary>
    /// Creates a zero money value with the specified currency
    /// </summary>
    /// <param name="currencyCode">The ISO 4217 currency code</param>
    /// <returns>A zero Money value object</returns>
    /// <exception cref="DomainException">Thrown when currency code is invalid</exception>
    public static Money Zero(string currencyCode)
    {
        return Create(0, currencyCode);
    }
    
    /// <summary>
    /// Adds two money values of the same currency
    /// </summary>
    /// <param name="other">The money to add</param>
    /// <returns>A new Money instance with the sum</returns>
    /// <exception cref="DomainException">Thrown when currencies don't match</exception>
    public Money Add(Money other)
    {
        if (other.CurrencyCode != CurrencyCode)
        {
            throw new DomainException($"Cannot add money with different currencies: {CurrencyCode} and {other.CurrencyCode}");
        }
        
        return new Money(Amount + other.Amount, CurrencyCode);
    }
    
    /// <summary>
    /// Subtracts another money value of the same currency
    /// </summary>
    /// <param name="other">The money to subtract</param>
    /// <returns>A new Money instance with the difference</returns>
    /// <exception cref="DomainException">Thrown when currencies don't match</exception>
    public Money Subtract(Money other)
    {
        if (other.CurrencyCode != CurrencyCode)
        {
            throw new DomainException($"Cannot subtract money with different currencies: {CurrencyCode} and {other.CurrencyCode}");
        }
        
        return new Money(Amount - other.Amount, CurrencyCode);
    }
    
    /// <summary>
    /// Multiplies the money amount by a factor
    /// </summary>
    /// <param name="factor">The multiplication factor</param>
    /// <returns>A new Money instance with the multiplied amount</returns>
    public Money Multiply(decimal factor)
    {
        return new Money(Amount * factor, CurrencyCode);
    }
    
    public override string ToString()
    {
        return $"{Amount} {CurrencyCode}";
    }
}
