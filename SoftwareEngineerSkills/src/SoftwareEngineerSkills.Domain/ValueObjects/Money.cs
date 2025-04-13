using SoftwareEngineerSkills.Domain.Common;

namespace SoftwareEngineerSkills.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount with a currency
/// </summary>
public sealed record Money : ValueObject
{
    private Money(decimal amount, string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code cannot be empty", nameof(currencyCode));
            
        if (currencyCode.Length != 3)
            throw new ArgumentException("Currency code must be 3 characters", nameof(currencyCode));
            
        Amount = amount;
        CurrencyCode = currencyCode.ToUpperInvariant();
    }

    /// <summary>
    /// The monetary amount
    /// </summary>
    public decimal Amount { get; }
    
    /// <summary>
    /// The ISO 4217 currency code (e.g., USD, EUR)
    /// </summary>
    public string CurrencyCode { get; }

    /// <summary>
    /// Creates a new Money value object
    /// </summary>
    /// <param name="amount">Monetary amount</param>
    /// <param name="currencyCode">ISO 4217 currency code (e.g., USD, EUR)</param>
    /// <returns>New Money instance</returns>
    public static Money Create(decimal amount, string currencyCode)
    {
        return new Money(amount, currencyCode);
    }

    /// <summary>
    /// Adds two monetary amounts of the same currency
    /// </summary>
    /// <param name="other">Money to add</param>
    /// <returns>New Money instance with the sum</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match</exception>
    public Money Add(Money other)
    {
        if (CurrencyCode != other.CurrencyCode)
            throw new InvalidOperationException($"Cannot add money with different currencies: {CurrencyCode} and {other.CurrencyCode}");
            
        return new Money(Amount + other.Amount, CurrencyCode);
    }

    /// <summary>
    /// Subtracts a monetary amount from this one (same currency)
    /// </summary>
    /// <param name="other">Money to subtract</param>
    /// <returns>New Money instance with the difference</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match</exception>
    public Money Subtract(Money other)
    {
        if (CurrencyCode != other.CurrencyCode)
            throw new InvalidOperationException($"Cannot subtract money with different currencies: {CurrencyCode} and {other.CurrencyCode}");
            
        return new Money(Amount - other.Amount, CurrencyCode);
    }

    public override string ToString() => $"{Amount} {CurrencyCode}";
}
