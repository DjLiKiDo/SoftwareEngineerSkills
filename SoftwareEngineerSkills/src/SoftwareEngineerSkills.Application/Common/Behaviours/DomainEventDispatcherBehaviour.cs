using MediatR;
using SoftwareEngineerSkills.Application.Common.Events;
using SoftwareEngineerSkills.Domain.Aggregates;
using SoftwareEngineerSkills.Domain.Common;
using SoftwareEngineerSkills.Domain.Common.Models;
using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Application.Common.Behaviours;

/// <summary>
/// MediatR pipeline behavior that automatically dispatches domain events after a command is processed
/// </summary>
/// <typeparam name="TRequest">The type of the request being handled</typeparam>
/// <typeparam name="TResponse">The type of the response from the handler</typeparam>
public class DomainEventDispatcherBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventDispatcherBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="domainEventDispatcher">The domain event dispatcher</param>
    public DomainEventDispatcherBehaviour(IDomainEventDispatcher domainEventDispatcher)
    {
        _domainEventDispatcher = domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
    }

    /// <summary>
    /// Pipeline handler which dispatches any domain events after the request is processed
    /// </summary>
    /// <param name="request">The request being processed</param>
    /// <param name="next">The next behavior in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response from the handler</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Process the request
        var response = await next();
        
        // Check if the response contains domain events that need to be dispatched
        if (response is Result result)
        {
            await DispatchDomainEventsFromResultAsync(result, cancellationToken);
        }
        else if (response is IResult genericResult)
        {
            // For generic Result<T>
            var resultType = genericResult.GetType();
            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                await DispatchDomainEventsFromResultAsync(genericResult, cancellationToken);
            }
        }

        return response;
    }

    private async Task DispatchDomainEventsFromResultAsync(object result, CancellationToken cancellationToken)
    {
        // Try to get domain events from the result if it contains any (e.g., from entities modified in the command)
        var domainEvents = new List<IDomainEvent>();

        // Collect domain events from the result properties
        DomainEventDispatcherBehaviour<TRequest, TResponse>.CollectDomainEvents(result, domainEvents);

        // Dispatch all collected domain events
        if (domainEvents.Any())
        {
            await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
        }
    }
    
    private static void CollectDomainEvents(object result, List<IDomainEvent> domainEvents)
    {
        // If the result has an Entity or Aggregate property, extract events from it
        var properties = result.GetType().GetProperties();
        
        foreach (var property in properties)
        {
            // Skip null properties
            var value = property.GetValue(result);
            if (value == null) continue;
            
            // Check if it's an entity with domain events
            if (value is Entity entity)
            {
                DomainEventDispatcherBehaviour<TRequest, TResponse>.ProcessEntityDomainEvents(entity, domainEvents);
            }
            
            // Check if it's an aggregate root
            else if (value is IAggregateRoot aggregate) 
            {
                DomainEventDispatcherBehaviour<TRequest, TResponse>.ProcessAggregateDomainEvents(aggregate, domainEvents);
            }
            
            // Check if it's a collection
            else if (value is IEnumerable<object> collection)
            {
                DomainEventDispatcherBehaviour<TRequest, TResponse>.ProcessCollectionDomainEvents(collection, domainEvents);
            }
        }
    }
    
    private static void ProcessEntityDomainEvents(Entity entity, List<IDomainEvent> domainEvents)
    {
        var entityEvents = GetDomainEventsFromEntity(entity);
        if (entityEvents.Any())
        {
            domainEvents.AddRange(entityEvents);
            entity.ClearDomainEvents();
        }
    }
    
    private static void ProcessAggregateDomainEvents(IAggregateRoot aggregate, List<IDomainEvent> domainEvents)
    {
        var aggregateEvents = GetDomainEventsFromAggregate(aggregate);
        if (aggregateEvents.Any())
        {
            domainEvents.AddRange(aggregateEvents);
            aggregate.ClearDomainEvents();
        }
    }
    
    private static void ProcessCollectionDomainEvents(IEnumerable<object> collection, List<IDomainEvent> domainEvents)
    {
        foreach (var item in collection)
        {
            if (item is Entity collectionEntity)
            {
                DomainEventDispatcherBehaviour<TRequest, TResponse>.ProcessEntityDomainEvents(collectionEntity, domainEvents);
            }
            else if (item is IAggregateRoot collectionAggregate)
            {
                DomainEventDispatcherBehaviour<TRequest, TResponse>.ProcessAggregateDomainEvents(collectionAggregate, domainEvents);
            }
        }
    }

    private static IEnumerable<IDomainEvent> GetDomainEventsFromEntity(Entity entity)
    {
        // Get domain events from the entity
        return entity.DomainEvents;
    }
    
    private static IEnumerable<IDomainEvent> GetDomainEventsFromAggregate(IAggregateRoot aggregate)
    {
        // Get domain events from the aggregate
        return aggregate.DomainEvents;
    }
}