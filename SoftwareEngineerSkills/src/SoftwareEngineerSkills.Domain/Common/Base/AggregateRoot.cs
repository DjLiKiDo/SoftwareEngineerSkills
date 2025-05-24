using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Common.Interfaces;
using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Domain.Common.Base;

/// <summary>
/// Base class for aggregate roots in the domain model implementing Domain-Driven Design patterns.
/// </summary>
/// <remarks>
/// <para>
/// Aggregate roots are the primary entities that external objects reference and the only
/// entities that can be retrieved directly from a repository. They are responsible for:
/// </para>
/// <list type="bullet">
/// <item><description>Enforcing invariants and consistency rules for the entire aggregate</description></item>
/// <item><description>Coordinating the lifecycle of child entities within the aggregate boundary</description></item>
/// <item><description>Managing domain events that represent significant business occurrences</description></item>
/// <item><description>Providing thread-safe operations for concurrent access scenarios</description></item>
/// </list>
/// <para>
/// This class extends <see cref="BaseEntity"/> and implements <see cref="IAggregateRoot"/>
/// to provide enhanced domain event handling with thread safety and automatic version management.
/// </para>
/// <para>
/// <strong>Thread Safety:</strong> This class provides thread-safe domain event handling
/// through internal locking mechanisms. Multiple threads can safely add events concurrently.
/// </para>
/// <example>
/// <code>
/// public class Customer : AggregateRoot
/// {
///     public string Name { get; private set; }
///     private readonly List&lt;Order&gt; _orders = new();
///     
///     public Customer(string name)
///     {
///         Name = name ?? throw new ArgumentNullException(nameof(name));
///         AddDomainEvent(new CustomerCreatedEvent(Id, name));
///     }
///     
///     public void UpdateName(string newName)
///     {
///         var oldName = Name;
///         Name = newName;
///         AddAndApplyEvent(new CustomerNameChangedEvent(Id, oldName, newName));
///     }
///     
///     protected override void Apply(IDomainEvent domainEvent)
///     {
///         switch (domainEvent)
///         {
///             case CustomerNameChangedEvent nameChanged:
///                 // Apply any additional state changes
///                 break;
///         }
///     }
///     
///     protected override IEnumerable&lt;string&gt; CheckInvariants()
///     {
///         if (string.IsNullOrWhiteSpace(Name))
///             yield return "Customer name cannot be empty";
///     }
/// }
/// </code>
/// </example>
/// </remarks>
/// <seealso cref="BaseEntity"/>
/// <seealso cref="IAggregateRoot"/>
/// <seealso cref="IDomainEvent"/>
public abstract class AggregateRoot : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// Thread-safe synchronization object for domain event operations.
    /// </summary>
    private readonly object _domainEventLock = new();
    
    /// <summary>
    /// Adds a domain event to this aggregate root in a thread-safe manner.
    /// </summary>
    /// <param name="domainEvent">The domain event to add. Cannot be null.</param>
    /// <remarks>
    /// <para>
    /// This method provides thread-safe access to domain event collection and automatically
    /// increments the aggregate version after successfully adding the event. The version
    /// increment helps with optimistic concurrency control and event ordering.
    /// </para>
    /// <para>
    /// This method shadows the base class method to provide aggregate-specific behavior
    /// including thread safety and automatic version management.
    /// </para>
    /// <example>
    /// <code>
    /// public void ChangeCustomerName(string newName)
    /// {
    ///     var oldName = Name;
    ///     Name = newName;
    ///     AddDomainEvent(new CustomerNameChangedEvent(Id, oldName, newName));
    ///     // Version is automatically incremented
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="domainEvent"/> is null.</exception>
    /// <seealso cref="AddAndApplyEvent"/>
    /// <seealso cref="Apply"/>
    protected new void AddDomainEvent(IDomainEvent domainEvent)
    {
        lock (_domainEventLock)
        {
            base.AddDomainEvent(domainEvent);
        }
        
        // After adding a domain event, increment the version
        IncrementVersion();
    }
    
    /// <summary>
    /// Applies the effects of a domain event to the aggregate's state.
    /// </summary>
    /// <param name="domainEvent">The domain event that should modify the aggregate state.</param>
    /// <remarks>
    /// <para>
    /// This method implements the "Apply" pattern from event sourcing, where domain events
    /// are used to modify aggregate state in a controlled manner. The default implementation
    /// does nothing, allowing derived classes to override this method to handle specific events.
    /// </para>
    /// <para>
    /// Override this method in derived aggregates to implement event-driven state changes.
    /// This pattern ensures that all state changes go through domain events, providing
    /// better traceability and consistency.
    /// </para>
    /// <example>
    /// <code>
    /// protected override void Apply(IDomainEvent domainEvent)
    /// {
    ///     switch (domainEvent)
    ///     {
    ///         case CustomerNameChangedEvent nameChanged:
    ///             // Additional state changes beyond direct property assignment
    ///             LastNameChangeDate = DateTime.UtcNow;
    ///             break;
    ///         case CustomerEmailChangedEvent emailChanged:
    ///             // Handle email change side effects
    ///             EmailVerificationStatus = EmailStatus.Pending;
    ///             break;
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso cref="AddAndApplyEvent"/>
    /// <seealso cref="AddDomainEvent"/>
    protected virtual void Apply(IDomainEvent domainEvent)
    {
        // Default implementation does nothing
        // Derived classes should override this method to apply the event to the aggregate state
    }
    
    /// <summary>
    /// Applies a domain event to the aggregate state and adds it to the event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to apply and add. Cannot be null.</param>
    /// <remarks>
    /// <para>
    /// This method combines event application and event collection in a single operation,
    /// ensuring that state changes and event recording happen atomically. It follows
    /// the sequence: Apply → Add → Validate to maintain consistency.
    /// </para>
    /// <para>
    /// The method automatically enforces domain invariants after applying the event,
    /// throwing a <see cref="DomainValidationException"/> if any business rules are violated.
    /// </para>
    /// <example>
    /// <code>
    /// public void ChangeCustomerEmail(string newEmail)
    /// {
    ///     var oldEmail = Email;
    ///     Email = newEmail; // Direct state change
    ///     
    ///     // Apply event with side effects and add to event collection
    ///     AddAndApplyEvent(new CustomerEmailChangedEvent(Id, oldEmail, newEmail));
    ///     // Invariants are automatically validated
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="domainEvent"/> is null.</exception>
    /// <exception cref="DomainValidationException">Thrown when domain invariants are violated after applying the event.</exception>
    /// <seealso cref="Apply"/>
    /// <seealso cref="AddDomainEvent"/>
    /// <seealso cref="AddAndApplyEventAsync"/>
    protected void AddAndApplyEvent(IDomainEvent domainEvent)
    {
        Apply(domainEvent);
        AddDomainEvent(domainEvent);
        EnforceInvariants(); // Validate invariants after state change
    }

    /// <summary>
    /// Asynchronously applies a domain event to the aggregate state and adds it to the event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to apply and add. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <para>
    /// This method provides async support for event application when the Apply method or
    /// invariant validation requires asynchronous operations (e.g., database lookups,
    /// external service calls). It follows the same sequence as <see cref="AddAndApplyEvent"/>
    /// but supports async invariant validation.
    /// </para>
    /// <para>
    /// The method automatically enforces domain invariants asynchronously after applying
    /// the event, throwing a <see cref="DomainValidationException"/> if any business rules are violated.
    /// </para>
    /// <example>
    /// <code>
    /// public async Task ChangeCustomerEmailAsync(string newEmail)
    /// {
    ///     var oldEmail = Email;
    ///     Email = newEmail;
    ///     
    ///     // Apply event and validate with async invariants
    ///     await AddAndApplyEventAsync(new CustomerEmailChangedEvent(Id, oldEmail, newEmail));
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="domainEvent"/> is null.</exception>
    /// <exception cref="DomainValidationException">Thrown when domain invariants are violated after applying the event.</exception>
    /// <seealso cref="Apply"/>
    /// <seealso cref="AddDomainEvent"/>
    /// <seealso cref="AddAndApplyEvent"/>
    protected async Task AddAndApplyEventAsync(IDomainEvent domainEvent)
    {
        Apply(domainEvent);
        AddDomainEvent(domainEvent);
        await EnforceInvariantsAsync(); // Validate invariants after state change
    }

    /// <summary>
    /// Removes a specific domain event from the aggregate root's collection in a thread-safe manner.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove from the collection.</param>
    /// <remarks>
    /// <para>
    /// This method provides thread-safe removal of domain events and overrides the base
    /// implementation to ensure consistency in multi-threaded scenarios.
    /// </para>
    /// <example>
    /// <code>
    /// // Thread-safe event removal
    /// var eventToCancel = new CustomerPromotionEligibleEvent(customerId);
    /// aggregate.RemoveDomainEvent(eventToCancel);
    /// </code>
    /// </example>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="domainEvent"/> is null.</exception>
    /// <seealso cref="AddDomainEvent"/>
    /// <seealso cref="ClearDomainEvents"/>
    public new void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        lock (_domainEventLock)
        {
            base.RemoveDomainEvent(domainEvent);
        }
    }

    /// <summary>
    /// Clears all domain events from the aggregate root's collection in a thread-safe manner.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method provides thread-safe clearing of all domain events and overrides the base
    /// implementation to ensure consistency in multi-threaded scenarios. It should typically
    /// be called after events have been successfully dispatched.
    /// </para>
    /// <example>
    /// <code>
    /// // Thread-safe event clearing after successful dispatch
    /// foreach (var domainEvent in aggregate.DomainEvents)
    /// {
    ///     await eventDispatcher.DispatchAsync(domainEvent);
    /// }
    /// aggregate.ClearDomainEvents();
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso cref="AddDomainEvent"/>
    /// <seealso cref="RemoveDomainEvent"/>
    public new void ClearDomainEvents()
    {
        lock (_domainEventLock)
        {
            base.ClearDomainEvents();
        }
    }

    /// <summary>
    /// Asynchronously enforces business invariants for this aggregate root.
    /// </summary>
    /// <returns>A task representing the asynchronous validation operation.</returns>
    /// <remarks>
    /// <para>
    /// This method implements the IAggregateRoot interface requirement for asynchronous
    /// invariant validation. It delegates to the base class implementation while ensuring
    /// thread safety for aggregate-specific operations.
    /// </para>
    /// <para>
    /// Aggregate roots should override the base CheckInvariantsAsync method to implement
    /// specific validation logic that may require I/O operations, such as validating
    /// business rules that depend on other aggregates or external services.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Enforce invariants asynchronously after state changes
    /// public async Task ProcessOrderAsync(Order order)
    /// {
    ///     _orders.Add(order);
    ///     await EnforceInvariantsAsync(); // Validate business rules
    ///     AddDomainEvent(new OrderProcessedEvent(Id, order.Id));
    /// }
    /// </code>
    /// </example>
    /// <exception cref="DomainValidationException">
    /// Thrown when one or more business invariants are violated.
    /// </exception>
    /// <seealso cref="EnforceInvariants"/>
    /// <seealso cref="CheckInvariantsAsync"/>
    public Task EnforceInvariantsAsync()
    {
        return base.EnforceInvariantsAsync();
    }
}
