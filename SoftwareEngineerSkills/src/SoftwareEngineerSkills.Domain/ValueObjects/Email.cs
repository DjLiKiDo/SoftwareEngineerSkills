namespace SoftwareEngineerSkills.Domain.ValueObjects;

/// <summary>
/// Represents an email address as a value object
/// </summary>
public sealed class Email
{
    /// <summary>
    /// Gets the value of the email address
    /// </summary>
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new Email value object
    /// </summary>
    /// <param name="email">The email address string</param>
    /// <returns>A new Email value object</returns>
    /// <exception cref="ArgumentException">Thrown when the email is invalid</exception>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("Email is not in a valid format", nameof(email));

        return new Email(email);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is Email other)
        {
            return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();
}
