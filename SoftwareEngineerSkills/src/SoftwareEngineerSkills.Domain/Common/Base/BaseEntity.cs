using System.ComponentModel.DataAnnotations.Schema;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Common.Interfaces;
using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Domain.Common.Base;

/// <summary>
/// Abstract base class for all entities in the domain model.
/// Provides common functionality including unique identification, auditing,
/// version control for optimistic concurrency, domain event handling,
/// and invariant validation.
/// </summary>
/// <remarks>
/// <para>
/// This class implements the foundation for Domain-Driven Design entities:
/// - Unique identity through GUID-based Id property
/// - Audit tracking through IAuditableEntity interface
/// - Domain event support for cross-aggregate communication
/// - Invariant validation to enforce business rules
/// - Version control for optimistic concurrency
/// </para>
/// <para>
/// Thread Safety:
/// This class is not thread-safe. Domain events collection modifications
/// should be synchronized by derived classes if accessed from multiple threads.
/// </para>
/// <para>
/// Usage:
/// Entities should inherit from this class to gain standard domain functionality.
/// Override CheckInvariants() to implement business rule validation.
/// </para>
/// <example>
/// <code>
/// public class Customer : BaseEntity
/// {
///     public string Name { get; private set; }
///     public Email EmailAddress { get; private set; }
///     
///     protected override void CheckInvariants()
///     {
///         if (string.IsNullOrWhiteSpace(Name))
///             throw new DomainValidationException("Customer name cannot be empty");
///             
///         if (EmailAddress == null)
///             throw new DomainValidationException("Customer must have an email address");
///     }
///     
///     public void UpdateEmail(Email newEmail)
///     {
///         EmailAddress = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
///         EnforceInvariants(); // Validate business rules after state change
///         AddDomainEvent(new CustomerEmailUpdatedEvent(Id, newEmail));
///     }
/// }
/// </code>
/// </example>
/// </remarks>
/// <seealso cref="IAuditableEntity"/>
/// <seealso cref="IDomainEvent"/>
/// <seealso cref="AggregateRoot"/>
public abstract class BaseEntity : IAuditableEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets the unique identifier for this entity.
    /// </summary>
    /// <value>
    /// A GUID that uniquely identifies this entity instance across the entire system.
    /// </value>
    /// <remarks>
    /// <para>
    /// The entity identifier is automatically generated when the entity is created
    /// and remains immutable throughout the entity's lifetime. This follows the
    /// Domain-Driven Design principle that entities are distinguished by their identity
    /// rather than their attributes.
    /// </para>
    /// <para>
    /// Using GUIDs provides globally unique identifiers that can be generated
    /// on the client side without requiring database round-trips, making the system
    /// more resilient and supporting distributed scenarios.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var customer = new Customer("John Doe", new Email("john@example.com"));
    /// var customerId = customer.Id; // GUID automatically assigned
    /// 
    /// // Identity comparison
    /// if (customer.Id == otherCustomer.Id)
    /// {
    ///     // Same entity instance
    /// }
    /// </code>
    /// </example>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    /// <summary>
    /// Gets the version number for optimistic concurrency control.
    /// </summary>
    /// <value>
    /// An integer representing the current version of this entity instance.
    /// </value>
    /// <remarks>
    /// <para>
    /// The version is used to implement optimistic concurrency control, preventing
    /// lost updates when multiple users attempt to modify the same entity simultaneously.
    /// Entity Framework uses this property as a concurrency token to detect conflicts.
    /// </para>
    /// <para>
    /// The version is automatically incremented each time the entity is saved to the database.
    /// When a conflict is detected (version mismatch), a DbUpdateConcurrencyException is thrown.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Load entity with current version
    /// var customer = await repository.GetByIdAsync(customerId);
    /// var originalVersion = customer.Version;
    /// 
    /// // Modify entity
    /// customer.UpdateEmail(new Email("newemail@example.com"));
    /// 
    /// // Save changes - version will be automatically incremented
    /// await unitOfWork.SaveChangesAsync();
    /// 
    /// // Version has been incremented
    /// Assert.Equal(originalVersion + 1, customer.Version);
    /// </code>
    /// </example>
    /// <seealso cref="IncrementVersion"/>
    public int Version { get; private set; }

    /// <summary>
    /// Gets the date and time when this entity was created.
    /// </summary>
    /// <value>
    /// A DateTime representing when this entity was first persisted to the database.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property is automatically set by the persistence layer when the entity
    /// is first saved. It provides an audit trail for tracking when entities were created.
    /// </para>
    /// <para>
    /// The value is set once during entity creation and remains immutable thereafter,
    /// providing a reliable timestamp for audit and business logic purposes.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var customer = new Customer("John Doe", new Email("john@example.com"));
    /// await repository.AddAsync(customer);
    /// await unitOfWork.SaveChangesAsync();
    /// 
    /// // Created timestamp is now set
    /// Console.WriteLine($"Customer created on: {customer.Created}");
    /// </code>
    /// </example>
    /// <seealso cref="IAuditableEntity"/>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets the identifier of the user who created this entity.
    /// </summary>
    /// <value>
    /// A string representing the user identifier who created this entity,
    /// or null if not available.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property provides audit trail information about who created the entity.
    /// It is automatically populated by the persistence layer using the current
    /// user context during entity creation.
    /// </para>
    /// <para>
    /// The value format depends on the authentication system being used
    /// (e.g., username, user ID, email address).
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Assuming user "admin@company.com" is logged in
    /// var customer = new Customer("John Doe", new Email("john@example.com"));
    /// await repository.AddAsync(customer);
    /// await unitOfWork.SaveChangesAsync();
    /// 
    /// // CreatedBy is automatically set
    /// Console.WriteLine($"Created by: {customer.CreatedBy}"); // Output: admin@company.com
    /// </code>
    /// </example>
    /// <seealso cref="IAuditableEntity"/>
    /// <seealso cref="LastModifiedBy"/>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets the date and time when this entity was last modified.
    /// </summary>
    /// <value>
    /// A DateTime representing when this entity was last updated in the database,
    /// or null if never modified since creation.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property is automatically updated by the persistence layer whenever
    /// the entity is saved with changes. It provides an audit trail for tracking
    /// when entities were last modified.
    /// </para>
    /// <para>
    /// For newly created entities that haven't been modified, this property
    /// remains null to distinguish between creation and modification events.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var customer = await repository.GetByIdAsync(customerId);
    /// customer.UpdateEmail(new Email("newemail@example.com"));
    /// await unitOfWork.SaveChangesAsync();
    /// 
    /// // LastModified timestamp is now set
    /// Console.WriteLine($"Last modified on: {customer.LastModified}");
    /// </code>
    /// </example>
    /// <seealso cref="IAuditableEntity"/>
    /// <seealso cref="Created"/>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// Gets the identifier of the user who last modified this entity.
    /// </summary>
    /// <value>
    /// A string representing the user identifier who last modified this entity,
    /// or null if never modified or user information is not available.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property provides audit trail information about who last modified the entity.
    /// It is automatically updated by the persistence layer using the current
    /// user context during entity updates.
    /// </para>
    /// <para>
    /// For newly created entities that haven't been modified, this property
    /// remains null. The value format depends on the authentication system being used.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Assuming user "editor@company.com" is logged in
    /// var customer = await repository.GetByIdAsync(customerId);
    /// customer.UpdateEmail(new Email("newemail@example.com"));
    /// await unitOfWork.SaveChangesAsync();
    /// 
    /// // LastModifiedBy is automatically updated
    /// Console.WriteLine($"Last modified by: {customer.LastModifiedBy}"); // Output: editor@company.com
    /// </code>
    /// </example>
    /// <seealso cref="IAuditableEntity"/>
    /// <seealso cref="CreatedBy"/>
    public string? LastModifiedBy { get; set; }

    /// <summary>
    /// Gets the collection of domain events associated with this entity.
    /// </summary>
    /// <value>
    /// A read-only collection of domain events that have been raised by this entity.
    /// </value>
    /// <remarks>
    /// <para>
    /// Domain events represent significant business occurrences within the entity's lifecycle.
    /// They enable loose coupling between different parts of the domain and support
    /// cross-aggregate communication without violating aggregate boundaries.
    /// </para>
    /// <para>
    /// Events are typically processed after successful persistence to ensure
    /// consistency between the entity state and event notifications.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var customer = new Customer("John Doe", new Email("john@example.com"));
    /// customer.UpdateEmail(new Email("newemail@example.com"));
    /// 
    /// // Check for domain events
    /// foreach (var domainEvent in customer.DomainEvents)
    /// {
    ///     if (domainEvent is CustomerEmailUpdatedEvent emailUpdated)
    ///     {
    ///         // Handle email update event
    ///         await emailNotificationService.SendConfirmation(emailUpdated.NewEmail);
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="AddDomainEvent"/>
    /// <seealso cref="ClearDomainEvents"/>
    /// <seealso cref="IDomainEvent"/>
    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Validates the business invariants of this entity.
    /// </summary>
    /// <returns>
    /// A collection of error messages describing any invariant violations.
    /// An empty collection indicates that all invariants are satisfied.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method should be overridden by derived classes to implement
    /// entity-specific business rule validation. It is called automatically
    /// by the <see cref="EnforceInvariants"/> method to ensure the entity
    /// remains in a consistent state.
    /// </para>
    /// <para>
    /// Unlike exception-based validation, this method returns error messages
    /// to allow for comprehensive validation where multiple invariants can be
    /// checked and reported simultaneously.
    /// </para>
    /// <para>
    /// This method is called synchronously and should not perform I/O operations.
    /// For asynchronous validation, use <see cref="CheckInvariantsAsync"/>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// protected override IEnumerable&lt;string&gt; CheckInvariants()
    /// {
    ///     if (string.IsNullOrWhiteSpace(Name))
    ///         yield return "Name cannot be empty";
    ///         
    ///     if (Name.Length > 100)
    ///         yield return "Name cannot exceed 100 characters";
    ///         
    ///     if (EmailAddress == null)
    ///         yield return "Customer must have an email address";
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="EnforceInvariants"/>
    /// <seealso cref="CheckInvariantsAsync"/>
    protected virtual IEnumerable<string> CheckInvariants()
    {
        // Default implementation - no invariants to check
        // Derived classes should override this method to implement specific business rules
        yield break;
    }

    /// <summary>
    /// Asynchronously validates the business invariants of this entity.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous validation operation.
    /// The task result contains a collection of error messages describing any invariant violations.
    /// An empty collection indicates that all invariants are satisfied.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method should be overridden by derived classes to implement
    /// entity-specific business rule validation that requires I/O operations,
    /// such as database queries or external service calls.
    /// </para>
    /// <para>
    /// This method is called automatically by the <see cref="EnforceInvariantsAsync"/>
    /// method to ensure the entity remains in a consistent state when asynchronous
    /// validation is required.
    /// </para>
    /// <para>
    /// The default implementation calls the synchronous <see cref="CheckInvariants"/>
    /// method and returns its results asynchronously.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// protected override async Task&lt;IEnumerable&lt;string&gt;&gt; CheckInvariantsAsync(CancellationToken cancellationToken = default)
    /// {
    ///     var errors = new List&lt;string&gt;();
    ///     errors.AddRange(CheckInvariants());
    ///     
    ///     // Perform async validation
    ///     if (EmailAddress != null)
    ///     {
    ///         var emailExists = await emailValidationService.IsEmailInUseAsync(EmailAddress.Value, cancellationToken);
    ///         if (emailExists)
    ///             errors.Add("Email address is already in use");
    ///     }
    ///     
    ///     return errors;
    /// }
    /// </code>
    /// </example>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    /// <seealso cref="EnforceInvariantsAsync"/>
    /// <seealso cref="CheckInvariants"/>
    protected virtual Task<IEnumerable<string>> CheckInvariantsAsync(CancellationToken cancellationToken = default)
    {
        // Default implementation - call synchronous validation and return results
        return Task.FromResult(CheckInvariants());
    }

    /// <summary>
    /// Enforces the business invariants of this entity by calling <see cref="CheckInvariants"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method provides a public interface for validating entity business rules.
    /// It should be called after any operation that modifies the entity state
    /// to ensure the entity remains in a valid state.
    /// </para>
    /// <para>
    /// The method delegates to the protected <see cref="CheckInvariants"/> method,
    /// which can be overridden by derived classes to implement specific validation logic.
    /// If any invariants are violated, a <see cref="DomainValidationException"/> is thrown
    /// containing all validation error messages.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public void UpdateEmail(Email newEmail)
    /// {
    ///     EmailAddress = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
    ///     EnforceInvariants(); // Validate after state change
    ///     AddDomainEvent(new CustomerEmailUpdatedEvent(Id, newEmail));
    /// }
    /// </code>
    /// </example>
    /// <exception cref="DomainValidationException">Thrown when validation rules are violated.</exception>
    /// <seealso cref="CheckInvariants"/>
    /// <seealso cref="EnforceInvariantsAsync"/>
    public void EnforceInvariants()
    {
        var errors = CheckInvariants().ToList();
        if (errors.Any())
        {
            throw new DomainValidationException(errors);
        }
    }

    /// <summary>
    /// Asynchronously enforces the business invariants of this entity by calling <see cref="CheckInvariantsAsync"/>.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous validation operation.</returns>
    /// <remarks>
    /// <para>
    /// This method provides a public interface for asynchronously validating entity business rules.
    /// It should be called after any operation that modifies the entity state when validation
    /// requires I/O operations such as database queries or external service calls.
    /// </para>
    /// <para>
    /// The method delegates to the protected <see cref="CheckInvariantsAsync"/> method,
    /// which can be overridden by derived classes to implement specific asynchronous validation logic.
    /// If any invariants are violated, a <see cref="DomainValidationException"/> is thrown
    /// containing all validation error messages.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public async Task UpdateEmailAsync(Email newEmail, CancellationToken cancellationToken = default)
    /// {
    ///     EmailAddress = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
    ///     await EnforceInvariantsAsync(cancellationToken); // Async validation
    ///     AddDomainEvent(new CustomerEmailUpdatedEvent(Id, newEmail));
    /// }
    /// </code>
    /// </example>
    /// <exception cref="DomainValidationException">Thrown when validation rules are violated.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    /// <seealso cref="CheckInvariantsAsync"/>
    /// <seealso cref="EnforceInvariants"/>
    public async Task EnforceInvariantsAsync(CancellationToken cancellationToken = default)
    {
        var errors = (await CheckInvariantsAsync(cancellationToken)).ToList();
        if (errors.Any())
        {
            throw new DomainValidationException(errors);
        }
    }

    /// <summary>
    /// Increments the entity version number for optimistic concurrency control.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is automatically called by the persistence layer during save operations
    /// to implement optimistic concurrency control. Each time an entity is saved, its version
    /// number is incremented to detect concurrent modifications.
    /// </para>
    /// <para>
    /// The version field is used by Entity Framework's concurrency tokens to prevent lost
    /// updates when multiple users attempt to modify the same entity simultaneously.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Called automatically by DbContext.SaveChanges()
    /// entity.IncrementVersion();
    /// 
    /// // Version-aware operations
    /// if (entity.Version != expectedVersion)
    /// {
    ///     throw new ConcurrencyException("Entity was modified by another user");
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="Version"/>
    public void IncrementVersion()
    {
        Version++;
    }

    /// <summary>
    /// Adds a domain event to this entity's event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    /// <remarks>
    /// <para>
    /// Domain events represent significant business occurrences and enable
    /// loose coupling between different parts of the domain. Events are typically
    /// processed after successful persistence to ensure consistency.
    /// </para>
    /// <para>
    /// Events should be added when the entity undergoes significant state changes
    /// that other parts of the system need to be aware of.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public void ChangeEmail(Email newEmail)
    /// {
    ///     var oldEmail = EmailAddress;
    ///     EmailAddress = newEmail;
    ///     
    ///     AddDomainEvent(new CustomerEmailChangedEvent(Id, oldEmail, newEmail));
    /// }
    /// </code>
    /// </example>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="domainEvent"/> is null.</exception>
    /// <seealso cref="DomainEvents"/>
    /// <seealso cref="RemoveDomainEvent"/>
    /// <seealso cref="ClearDomainEvents"/>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a specific domain event from this entity's event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    /// <returns>true if the event was successfully removed; otherwise, false.</returns>
    /// <remarks>
    /// <para>
    /// This method allows for selective removal of domain events, which can be useful
    /// in scenarios where certain events should be cancelled or replaced before processing.
    /// </para>
    /// <para>
    /// Returns false if the event was not found in the collection, indicating
    /// it was either never added or already removed.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var emailEvent = new CustomerEmailChangedEvent(Id, oldEmail, newEmail);
    /// AddDomainEvent(emailEvent);
    /// 
    /// // Later, if we need to cancel the event
    /// bool removed = RemoveDomainEvent(emailEvent);
    /// if (removed)
    /// {
    ///     Console.WriteLine("Email change event cancelled");
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="AddDomainEvent"/>
    /// <seealso cref="ClearDomainEvents"/>
    /// <seealso cref="DomainEvents"/>
    public bool RemoveDomainEvent(IDomainEvent domainEvent)
    {
        return _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Removes all domain events from this entity's event collection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is typically called by the persistence layer after domain events
    /// have been successfully processed to prevent them from being processed again
    /// on subsequent save operations.
    /// </para>
    /// <para>
    /// It can also be used to reset the entity's event state in testing scenarios
    /// or when events need to be discarded for business reasons.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // After processing all domain events
    /// foreach (var domainEvent in entity.DomainEvents)
    /// {
    ///     await eventDispatcher.DispatchAsync(domainEvent);
    /// }
    /// 
    /// // Clear events to prevent reprocessing
    /// entity.ClearDomainEvents();
    /// </code>
    /// </example>
    /// <seealso cref="AddDomainEvent"/>
    /// <seealso cref="RemoveDomainEvent"/>
    /// <seealso cref="DomainEvents"/>
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
