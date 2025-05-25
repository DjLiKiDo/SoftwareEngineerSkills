namespace SoftwareEngineerSkills.Domain.Common.Base;

/// <summary>
/// Abstract base class for value objects in Domain-Driven Design.
/// Value objects are immutable and defined by their attributes rather than identity.
/// </summary>
/// <remarks>
/// Examples: money amounts, date ranges, addresses, email addresses.
/// Two value objects are equal if all their attributes are equal.
/// </remarks>
public abstract class ValueObject
{
    /// <summary>
    /// Returns the component values that participate in equality comparison.
    /// </summary>
    /// <returns>An enumerable of objects representing the properties that define equality.</returns>
    /// <remarks>
    /// Include all properties that define the value object's identity. Order matters for equality.
    /// Use yield return for efficient enumeration.
    /// </remarks>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    /// Determines whether the specified object is equal to the current value object.
    /// </summary>
    /// <param name="obj">The object to compare with the current value object.</param>
    /// <returns>true if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Returns a hash code for the current value object.
    /// </summary>
    /// <returns>A hash code calculated from all equality components.</returns>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// Determines whether two value objects are equal.
    /// </summary>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
        {
            return true;
        }
        
        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
        {
            return false;
        }
        
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two value objects are not equal.
    /// </summary>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}
