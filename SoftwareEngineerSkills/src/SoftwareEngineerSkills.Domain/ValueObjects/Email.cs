using System.Text.RegularExpressions;
using SoftwareEngineerSkills.Domain.Common;

namespace SoftwareEngineerSkills.Domain.ValueObjects;

/// <summary>
/// Represents a valid email address
/// </summary>
public sealed record Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the email address value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new Email value object, validating the format
    /// </summary>
    /// <param name="email">Email address string to validate</param>
    /// <returns>New Email instance or throws exception if invalid</returns>
    /// <exception cref="ArgumentException">Thrown when email format is invalid</exception>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
            
        if (email.Length > 320)
            throw new ArgumentException("Email is too long", nameof(email));
            
        if (!EmailRegex.IsMatch(email))
            throw new ArgumentException("Email format is invalid", nameof(email));
            
        return new Email(email);
    }

    public static implicit operator string(Email email) => email.Value;
    
    public override string ToString() => Value;
}
