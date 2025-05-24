using SoftwareEngineerSkills.Domain.Common.Events;

namespace SoftwareEngineerSkills.Domain.Common.Interfaces;

/// <summary>
/// Defines the contract for aggregate roots in Domain-Driven Design.
/// </summary>
/// <remarks>
/// <para>
/// Aggregate roots are the only entities in an aggregate that external objects
/// are allowed to reference. They serve as the consistency boundary for the aggregate
/// and are responsible for:
/// </para>
/// <list type="bullet">
/// <item><description><strong>Identity:</strong> Providing unique identification for the aggregate</description></item>
/// <item><description><strong>Invariant Enforcement:</strong> Ensuring all business rules are satisfied</description></item>
/// <item><description><strong>Event Management:</strong> Coordinating domain events within the aggregate</description></item>
/// <item><description><strong>Lifecycle Management:</strong> Controlling the creation and modification of child entities</description></item>
/// <item><description><strong>Concurrency Control:</strong> Managing optimistic concurrency through versioning</description></item>
/// </list>
/// <para>
/// <strong>Implementation Guidelines:</strong>
/// </para>
/// <list type="number">
/// <item><description>Inherit from <see cref="AggregateRoot"/> base class for automatic implementation</description></item>
/// <item><description>Use this interface to identify aggregate roots in repository patterns</description></item>
/// <item><description>Encapsulate all state changes through domain events</description></item>
/// <item><description>Apply changes through events to maintain consistency</description></item>
/// <item><description>Validate invariants after every state change</description></item>
/// </list>
/// <para>
/// <strong>Repository Usage:</strong> Only aggregate roots should have repositories.
/// Child entities should be accessed through their aggregate root.
/// </para>
/// <example>
/// <code>
/// // Repository for aggregate roots only
/// public interface IRepository&lt;T&gt; where T : class, IAggregateRoot
/// {
///     Task&lt;T?&gt; GetByIdAsync(Guid id);
///     Task AddAsync(T aggregate);
///     void Update(T aggregate);
///     void Remove(T aggregate);
/// }
/// 
/// // Example usage
/// public class CustomerRepository : IRepository&lt;Customer&gt;
/// {
///     // Implementation for Customer aggregate root
/// }
/// </code>
/// </example>
/// </remarks>
/// <seealso cref="AggregateRoot"/>
/// <seealso cref="IDomainEvent"/>
/// <seealso href="https://martinfowler.com/bliki/DDD_Aggregate.html">Martin Fowler - DDD Aggregate</seealso>
public interface IAggregateRoot
{
    /// <summary>
    /// Gets the collection of domain events that have been raised by this aggregate root.
    /// </summary>
    /// <value>
    /// A read-only collection of domain events representing significant business occurrences
    /// within this aggregate that other parts of the system may need to react to.
    /// </value>
    /// <remarks>
    /// <para>
    /// Domain events capture important business happenings and are used to integrate
    /// different parts of the system in a loosely coupled manner. They should be
    /// dispatched after successful persistence to ensure eventual consistency.
    /// </para>
    /// <para>
    /// The collection should be cleared after events are successfully dispatched
    /// to prevent duplicate processing.
    /// </para>
    /// <example>
    /// <code>
    /// // Process events after saving
    /// foreach (var domainEvent in aggregate.DomainEvents)
    /// {
    ///     await eventDispatcher.DispatchAsync(domainEvent);
    /// }
    /// aggregate.ClearDomainEvents();
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso cref="ClearDomainEvents"/>
    /// <seealso cref="RemoveDomainEvent"/>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    
    /// <summary>
    /// Gets the version number of this aggregate root for optimistic concurrency control.
    /// </summary>
    /// <value>
    /// An integer representing the current version of the aggregate. This value is
    /// incremented each time the aggregate is modified and saved.
    /// </value>
    /// <remarks>
    /// <para>
    /// The version property is used to implement optimistic concurrency control,
    /// preventing lost updates when multiple users attempt to modify the same
    /// aggregate simultaneously. Entity Framework uses this as a concurrency token.
    /// </para>
    /// <para>
    /// Version increments typically occur:
    /// </para>
    /// <list type="bullet">
    /// <item><description>When domain events are added to the aggregate</description></item>
    /// <item><description>During save operations in the persistence layer</description></item>
    /// <item><description>When significant state changes occur</description></item>
    /// </list>
    /// <example>
    /// <code>
    /// // Check for concurrent modifications
    /// var currentAggregate = await repository.GetByIdAsync(id);
    /// if (currentAggregate.Version != expectedVersion)
    /// {
    ///     throw new ConcurrencyException("Aggregate was modified by another user");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    int Version { get; }
    
    /// <summary>
    /// Removes a specific domain event from the aggregate root's event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove from the collection.</param>
    /// <remarks>
    /// <para>
    /// This method allows selective removal of domain events, which can be useful
    /// in scenarios where certain events should be cancelled based on subsequent
    /// business operations or when implementing complex event processing logic.
    /// </para>
    /// <example>
    /// <code>
    /// // Cancel a specific event if conditions change
    /// var eventToCancel = new CustomerPromotionEligibleEvent(customerId);
    /// if (!customer.MeetsPromotionCriteria())
    /// {
    ///     customer.RemoveDomainEvent(eventToCancel);
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso cref="ClearDomainEvents"/>
    /// <seealso cref="DomainEvents"/>
    void RemoveDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// Clears all domain events from the aggregate root's event collection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method should typically be called after domain events have been
    /// successfully dispatched to prevent duplicate processing. It's usually
    /// invoked by the infrastructure layer after persistence operations complete
    /// and events have been published.
    /// </para>
    /// <para>
    /// <strong>Important:</strong> Only clear events after they have been successfully
    /// processed to avoid losing important business events.
    /// </para>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     // Save aggregate and dispatch events
    ///     await repository.SaveAsync(aggregate);
    ///     
    ///     foreach (var domainEvent in aggregate.DomainEvents)
    ///     {
    ///         await eventDispatcher.DispatchAsync(domainEvent);
    ///     }
    ///     
    ///     // Clear events only after successful dispatch
    ///     aggregate.ClearDomainEvents();
    /// }
    /// catch (Exception)
    /// {
    ///     // Don't clear events if processing failed
    ///     throw;
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso cref="DomainEvents"/>
    /// <seealso cref="RemoveDomainEvent"/>
    void ClearDomainEvents();
    
    /// <summary>
    /// Validates and enforces all business invariants for this aggregate root.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method validates that the aggregate root is in a consistent state
    /// according to all defined business rules. It should be called after any
    /// operation that modifies the aggregate's state to ensure data integrity.
    /// </para>
    /// <para>
    /// If any invariants are violated, this method throws a <see cref="DomainValidationException"/>
    /// containing details about the validation failures.
    /// </para>
    /// <example>
    /// <code>
    /// public void UpdateCustomerCredit(decimal newCreditLimit)
    /// {
    ///     CreditLimit = newCreditLimit;
    ///     EnforceInvariants(); // Validate business rules after change
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <exception cref="DomainValidationException">
    /// Thrown when one or more business invariants are violated.
    /// </exception>
    /// <seealso cref="EnforceInvariantsAsync"/>
    void EnforceInvariants();
    
    /// <summary>
    /// Asynchronously validates and enforces all business invariants for this aggregate root.
    /// </summary>
    /// <returns>A task that represents the asynchronous validation operation.</returns>
    /// <remarks>
    /// <para>
    /// This method provides async support for invariant validation when business rules
    /// require asynchronous operations such as database lookups, external service calls,
    /// or complex validation logic that may take time to complete.
    /// </para>
    /// <para>
    /// Like <see cref="EnforceInvariants"/>, this method throws a <see cref="DomainValidationException"/>
    /// if any invariants are violated, but allows for async validation operations.
    /// </para>
    /// <example>
    /// <code>
    /// public async Task UpdateCustomerEmailAsync(string newEmail)
    /// {
    ///     Email = newEmail;
    ///     // Async validation may check email uniqueness in database
    ///     await EnforceInvariantsAsync();
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <exception cref="DomainValidationException">
    /// Thrown when one or more business invariants are violated.
    /// </exception>
    /// <seealso cref="EnforceInvariants"/>
    Task EnforceInvariantsAsync();
}
