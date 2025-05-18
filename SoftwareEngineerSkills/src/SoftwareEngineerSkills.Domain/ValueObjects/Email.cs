using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Domain.ValueObjects;

/// <summary>
/// ValueObject that represents an email address
/// </summary>
public class Email : ValueObject
{
    /// <summary>
    /// Gets the email address value
    /// </summary>
    public string Value { get; }
    
    // Private constructor for EF Core
    private Email() 
    {
        Value = string.Empty;
    }
    
    /// <summary>
    /// Creates a new instance of the Email value object
    /// </summary>
    /// <param name="value">The email address value</param>
    /// <exception cref="BusinessRuleException">Thrown when the email format is invalid</exception>
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleException("Email cannot be empty");
        }
        
        if (!IsValidEmail(value))
        {
            throw new BusinessRuleException("Email has an invalid format");
        }
        
        Value = value;
    }
    
    /// <summary>
    /// Validates if the given string is a valid email format
    /// </summary>
    /// <param name="email">The email to validate</param>
    /// <returns>True if the email is valid; otherwise, false</returns>
    private static bool IsValidEmail(string email)
    {
        // Basic regex for email validation
        if (!System.Text.RegularExpressions.Regex.IsMatch(email, 
            @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$"))
        {
            return false;
        }

        // Additional validations that System.Net.Mail.MailAddress might not catch
        var parts = email.Split('@');
        if (parts.Length != 2)
            return false;

        var domainParts = parts[1].Split('.');
        if (domainParts.Length < 2)
            return false;

        // Check if TLD is at least 2 characters
        if (domainParts[^1].Length < 2)
            return false;

        // Ensure no part is empty
        foreach (var part in domainParts)
        {
            if (string.IsNullOrEmpty(part))
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Gets the components of this value object that are used in equality comparisons
    /// </summary>
    /// <returns>The equality components of this value object</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value.ToLowerInvariant();
    }
    
    /// <summary>
    /// Implicitly converts an Email to a string
    /// </summary>
    /// <param name="email">The email to convert</param>
    public static implicit operator string(Email email) => email.Value;
    
    /// <summary>
    /// Explicitly converts a string to an Email
    /// </summary>
    /// <param name="email">The string to convert</param>
    public static explicit operator Email(string email) => new(email);
    
    /// <summary>
    /// Returns a string that represents the current object
    /// </summary>
    /// <returns>A string that represents the current object</returns>
    public override string ToString() => Value;
}
