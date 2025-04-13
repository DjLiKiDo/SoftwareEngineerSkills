using System;
using System.Collections.Generic;
using SoftwareEngineerSkills.Domain.Common.Models;

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
    /// <returns>A Result containing the new Money value object or an error</returns>
    public static Result<Money> Create(decimal amount, string currencyCode)
    {
        // Validate currency code (basic validation)
        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            return Result<Money>.Failure("Currency code cannot be empty");
        }
        
        if (currencyCode.Length != 3)
        {
            return Result<Money>.Failure("Currency code must be 3 characters (ISO 4217 format)");
        }
        
        // In a real application, we might validate against a list of valid currency codes
        
        return Result<Money>.Success(new Money(amount, currencyCode.ToUpperInvariant()));
    }
    
    /// <summary>
    /// Creates a zero money value with the specified currency
    /// </summary>
    /// <param name="currencyCode">The ISO 4217 currency code</param>
    /// <returns>A Result containing a zero Money value object or an error</returns>
    public static Result<Money> Zero(string currencyCode)
    {
        return Create(0, currencyCode);
    }
    
    /// <summary>
    /// Adds two money values of the same currency
    /// </summary>
    /// <param name="other">The money to add</param>
    /// <returns>A Result containing the sum or an error if currencies don't match</returns>
    public Result<Money> Add(Money other)
    {
        if (other.CurrencyCode != CurrencyCode)
        {
            return Result<Money>.Failure($"Cannot add money with different currencies: {CurrencyCode} and {other.CurrencyCode}");
        }
        
        return Result<Money>.Success(new Money(Amount + other.Amount, CurrencyCode));
    }
    
    /// <summary>
    /// Subtracts another money value of the same currency
    /// </summary>
    /// <param name="other">The money to subtract</param>
    /// <returns>A Result containing the difference or an error if currencies don't match</returns>
    public Result<Money> Subtract(Money other)
    {
        if (other.CurrencyCode != CurrencyCode)
        {
            return Result<Money>.Failure($"Cannot subtract money with different currencies: {CurrencyCode} and {other.CurrencyCode}");
        }
        
        return Result<Money>.Success(new Money(Amount - other.Amount, CurrencyCode));
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
