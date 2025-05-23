namespace SoftwareEngineerSkills.Domain.Common.Base;

/// <summary>
/// Abstract base class for value objects in Domain-Driven Design.
/// </summary>
/// <remarks>
/// <para>
/// Value objects are immutable objects that represent a descriptive aspect of the domain
/// with no conceptual identity. They are defined by their attributes rather than by an ID.
/// Examples include money amounts, date ranges, addresses, and email addresses.
/// </para>
/// <para>
/// Key characteristics of value objects:
/// </para>
/// <list type="bullet">
/// <item><description><strong>Immutability:</strong> Value objects should not change after creation</description></item>
/// <item><description><strong>Equality:</strong> Two value objects are equal if all their attributes are equal</description></item>
/// <item><description><strong>No Identity:</strong> Value objects are defined by their attributes, not by an ID</description></item>
/// <item><description><strong>Side-effect Free:</strong> Operations on value objects should not modify the object</description></item>
/// </list>
/// <para>
/// This base class provides automatic implementation of equality operations based on
/// the components returned by <see cref="GetEqualityComponents"/>. Derived classes
/// only need to implement this method to specify which properties participate in equality.
/// </para>
/// <example>
/// <code>
/// public class Money : ValueObject
/// {
///     public decimal Amount { get; }
///     public string Currency { get; }
///     
///     public Money(decimal amount, string currency)
///     {
///         Amount = amount;
///         Currency = currency ?? throw new ArgumentNullException(nameof(currency));
///     }
///     
///     protected override IEnumerable&lt;object&gt; GetEqualityComponents()
///     {
///         yield return Amount;
///         yield return Currency.ToUpperInvariant();
///     }
///     
///     public Money Add(Money other)
///     {
///         if (Currency != other.Currency)
///             throw new InvalidOperationException("Cannot add money with different currencies");
///         
///         return new Money(Amount + other.Amount, Currency);
///     }
/// }
/// </code>
/// </example>
/// </remarks>
/// <seealso href="https://martinfowler.com/bliki/ValueObject.html">Martin Fowler - Value Object</seealso>
public abstract class ValueObject
{
    /// <summary>
    /// When overridden in a derived class, returns the component values that participate in equality comparison.
    /// </summary>
    /// <returns>
    /// An enumerable collection of objects representing the properties or fields that define
    /// the identity of this value object for equality comparison purposes.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method is the foundation of value object equality. It should return all properties
    /// that determine whether two value objects are considered equal. The order of components
    /// matters for equality comparison.
    /// </para>
    /// <para>
    /// Guidelines for implementing this method:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Include all properties that define the value object's identity</description></item>
    /// <item><description>Use consistent ordering of components</description></item>
    /// <item><description>Consider case sensitivity for string values</description></item>
    /// <item><description>Handle null values appropriately</description></item>
    /// <item><description>Use yield return for efficient enumeration</description></item>
    /// </list>
    /// <example>
    /// <code>
    /// // Example for an Address value object
    /// protected override IEnumerable&lt;object&gt; GetEqualityComponents()
    /// {
    ///     yield return Street?.Trim()?.ToLowerInvariant() ?? string.Empty;
    ///     yield return City?.Trim()?.ToLowerInvariant() ?? string.Empty;
    ///     yield return PostalCode?.Trim()?.ToUpperInvariant() ?? string.Empty;
    ///     yield return Country?.Trim()?.ToLowerInvariant() ?? string.Empty;
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    /// Determines whether the specified object is equal to the current value object.
    /// </summary>
    /// <param name="obj">The object to compare with the current value object.</param>
    /// <returns>
    /// <c>true</c> if the specified object is equal to the current value object; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Two value objects are considered equal if:
    /// </para>
    /// <list type="number">
    /// <item><description>They are of the exact same type</description></item>
    /// <item><description>All their equality components are equal in the same order</description></item>
    /// </list>
    /// <para>
    /// This method automatically uses the components returned by <see cref="GetEqualityComponents"/>
    /// to perform deep equality comparison. Null handling is built-in.
    /// </para>
    /// <example>
    /// <code>
    /// var address1 = new Address("123 Main St", "Anytown", "12345", "USA");
    /// var address2 = new Address("123 Main St", "Anytown", "12345", "USA");
    /// var address3 = new Address("456 Oak St", "Anytown", "12345", "USA");
    /// 
    /// bool equal1 = address1.Equals(address2); // true
    /// bool equal2 = address1.Equals(address3); // false
    /// bool equal3 = address1.Equals(null);     // false
    /// bool equal4 = address1.Equals("string"); // false
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso cref="GetEqualityComponents"/>
    /// <seealso cref="GetHashCode"/>
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
    /// <returns>
    /// A hash code for the current value object, calculated from all equality components.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method automatically generates a hash code by combining the hash codes of all
    /// equality components returned by <see cref="GetEqualityComponents"/>. This ensures
    /// that value objects with the same component values will have the same hash code.
    /// </para>
    /// <para>
    /// The implementation uses XOR aggregation to combine component hash codes, providing
    /// reasonable distribution for hash-based collections. Null components are handled
    /// safely by using 0 as their hash code contribution.
    /// </para>
    /// <para>
    /// <strong>Important:</strong> This method must be consistent with <see cref="Equals(object)"/>.
    /// If two value objects are equal, they must have the same hash code.
    /// </para>
    /// <example>
    /// <code>
    /// var address1 = new Address("123 Main St", "Anytown", "12345", "USA");
    /// var address2 = new Address("123 Main St", "Anytown", "12345", "USA");
    /// 
    /// // These should be equal and have the same hash code
    /// bool areEqual = address1.Equals(address2);           // true
    /// bool sameHash = address1.GetHashCode() == address2.GetHashCode(); // true
    /// 
    /// // Can be used safely in hash-based collections
    /// var addressSet = new HashSet&lt;Address&gt; { address1 };
    /// bool contains = addressSet.Contains(address2);       // true
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso cref="Equals(object)"/>
    /// <seealso cref="GetEqualityComponents"/>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// Determines whether two value objects are equal using the equality operator.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>
    /// <c>true</c> if the value objects are equal; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This operator provides convenient syntax for value object equality comparison
    /// and handles null references safely. The comparison logic delegates to the
    /// <see cref="Equals(object)"/> method for non-null objects.
    /// </para>
    /// <para>
    /// Null handling rules:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Two null references are considered equal</description></item>
    /// <item><description>A null reference and a non-null reference are not equal</description></item>
    /// <item><description>Two non-null references are compared using <see cref="Equals(object)"/></description></item>
    /// </list>
    /// <example>
    /// <code>
    /// var money1 = new Money(100, "USD");
    /// var money2 = new Money(100, "USD");
    /// var money3 = new Money(200, "USD");
    /// Money money4 = null;
    /// Money money5 = null;
    /// 
    /// bool result1 = money1 == money2;  // true
    /// bool result2 = money1 == money3;  // false
    /// bool result3 = money1 == money4;  // false
    /// bool result4 = money4 == money5;  // true
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso cref="Equals(object)"/>
    /// <seealso cref="operator !=(ValueObject?, ValueObject?)"/>
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
    /// Determines whether two value objects are not equal using the inequality operator.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>
    /// <c>true</c> if the value objects are not equal; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This operator provides convenient syntax for value object inequality comparison.
    /// It simply negates the result of the equality operator (<see cref="operator ==(ValueObject?, ValueObject?)"/>).
    /// </para>
    /// <example>
    /// <code>
    /// var email1 = new Email("user@example.com");
    /// var email2 = new Email("admin@example.com");
    /// 
    /// bool different = email1 != email2;  // true
    /// bool same = email1 != email1;       // false
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso cref="operator ==(ValueObject?, ValueObject?)"/>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}
