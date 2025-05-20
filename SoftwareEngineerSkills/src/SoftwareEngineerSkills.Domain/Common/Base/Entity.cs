using SoftwareEngineerSkills.Domain.Common.Base;

namespace SoftwareEngineerSkills.Domain.Common.Base;

/// <summary>
/// Abstract base class for domain entities that serve as standard entities with identity.
/// This class provides equality comparison based on identity rather than structure.
/// Note: For entities that need to be aggregate roots, use AggregateRoot instead.
/// </summary>
public abstract class Entity : BaseEntity
{
    /// <summary>
    /// Determines whether this entity is equal to another entity.
    /// </summary>
    /// <param name="obj">The object to compare with the current entity.</param>
    /// <returns>True if the specified object is equal to this entity; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (Id == Guid.Empty || other.Id == Guid.Empty)
            return false;

        return Id == other.Id;
    }

    /// <summary>
    /// Serves as a hash function for the entity.
    /// </summary>
    /// <returns>A hash code for the current entity.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, GetType());
    }

    /// <summary>
    /// Equality operator overload.
    /// </summary>
    /// <param name="left">The left entity.</param>
    /// <param name="right">The right entity.</param>
    /// <returns>True if the entities are equal; otherwise, false.</returns>
    public static bool operator ==(Entity? left, Entity? right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;
        
        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;
        
        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator overload.
    /// </summary>
    /// <param name="left">The left entity.</param>
    /// <param name="right">The right entity.</param>
    /// <returns>True if the entities are not equal; otherwise, false.</returns>
    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}
