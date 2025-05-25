using System.ComponentModel.DataAnnotations.Schema;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Common.Interfaces;
using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Domain.Common.Base;

/// <summary>
/// Abstract base class for all domain entities providing identity, auditing, versioning, 
/// domain events, and invariant validation.
/// </summary>
/// <remarks>
/// Not thread-safe. Use AggregateRoot for thread-safe domain event handling.
/// Override CheckInvariants() to implement business rule validation.
/// </remarks>
public abstract class BaseEntity : IAuditableEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets the unique identifier for this entity.
    /// </summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    /// <summary>
    /// Gets the version number for optimistic concurrency control.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// Gets the date and time when this entity was created.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets the identifier of the user who created this entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets the date and time when this entity was last modified.
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// Gets the identifier of the user who last modified this entity.
    /// </summary>
    public string? LastModifiedBy { get; set; }

    /// <summary>
    /// Gets the collection of domain events associated with this entity.
    /// </summary>
    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Validates the business invariants of this entity.
    /// Override in derived classes to implement specific business rules.
    /// </summary>
    /// <returns>Collection of error messages. Empty if all invariants are satisfied.</returns>
    protected virtual IEnumerable<string> CheckInvariants()
    {
        yield break;
    }

    /// <summary>
    /// Asynchronously validates the business invariants of this entity.
    /// Override for validation requiring I/O operations.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Collection of error messages. Empty if all invariants are satisfied.</returns>
    protected virtual Task<IEnumerable<string>> CheckInvariantsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(CheckInvariants());
    }

    /// <summary>
    /// Enforces business invariants by calling CheckInvariants().
    /// Call after any state modification.
    /// </summary>
    /// <exception cref="DomainValidationException">Thrown when validation rules are violated.</exception>
    public void EnforceInvariants()
    {
        var errors = CheckInvariants().ToList();
        if (errors.Any())
        {
            throw new DomainValidationException(errors);
        }
    }

    /// <summary>
    /// Asynchronously enforces business invariants by calling CheckInvariantsAsync().
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <exception cref="DomainValidationException">Thrown when validation rules are violated.</exception>
    public async Task EnforceInvariantsAsync(CancellationToken cancellationToken = default)
    {
        var errors = (await CheckInvariantsAsync(cancellationToken)).ToList();
        if (errors.Any())
        {
            throw new DomainValidationException(errors);
        }
    }

    /// <summary>
    /// Increments the entity version for optimistic concurrency control.
    /// </summary>
    public void IncrementVersion()
    {
        Version++;
    }

    /// <summary>
    /// Adds a domain event to this entity's event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a specific domain event from this entity's event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    /// <returns>true if the event was removed; otherwise, false.</returns>
    public bool RemoveDomainEvent(IDomainEvent domainEvent)
    {
        return _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from this entity's event collection.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current entity.
    /// </summary>
    /// <param name="obj">The object to compare with the current entity.</param>
    /// <returns>true if the specified object is equal to the current entity; otherwise, false.</returns>
    /// <remarks>
    /// <para>
    /// Entity equality is based on the entity's identity (Id property) rather than
    /// its attribute values. Two entities are considered equal if they have the same
    /// type and the same Id, regardless of their other property values.
    /// </para>
    /// <para>
    /// This follows Domain-Driven Design principles where entities are distinguished
    /// by their identity rather than their attributes.
    /// </para>
    /// </remarks>
    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity other)
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
    /// Serves as the default hash function for the entity.
    /// </summary>
    /// <returns>A hash code for the current entity.</returns>
    /// <remarks>
    /// The hash code is based on the entity's Id and type to ensure consistency with
    /// the equality comparison implementation.
    /// </remarks>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, GetType());
    }

    /// <summary>
    /// Determines whether two entity instances are equal.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns>true if the entities are equal; otherwise, false.</returns>
    public static bool operator ==(BaseEntity? left, BaseEntity? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two entity instances are not equal.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns>true if the entities are not equal; otherwise, false.</returns>
    public static bool operator !=(BaseEntity? left, BaseEntity? right)
    {
        return !(left == right);
    }
}
