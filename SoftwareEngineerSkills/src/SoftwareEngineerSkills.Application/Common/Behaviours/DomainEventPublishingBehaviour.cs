using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Common;
using SoftwareEngineerSkills.Domain.Aggregates;

namespace SoftwareEngineerSkills.Application.Common.Behaviours;

/// <summary>
/// MediatR pipeline behavior that publishes domain events after request handling is complete
/// </summary>
/// <typeparam name="TRequest">The type of the request</typeparam>
/// <typeparam name="TResponse">The type of the response</typeparam>
public class DomainEventPublishingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IPublisher _publisher;
    private readonly ILogger<DomainEventPublishingBehaviour<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventPublishingBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="publisher">MediatR publisher</param>
    /// <param name="logger">Logger</param>
    public DomainEventPublishingBehaviour(
        IPublisher publisher,
        ILogger<DomainEventPublishingBehaviour<TRequest, TResponse>> logger)
    {
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Pipeline handler that publishes domain events after request handling is complete
    /// </summary>
    /// <param name="request">The request instance</param>
    /// <param name="next">The delegate for the next action in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response from the next handler in the pipeline</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Continue the request pipeline to get a response
        var response = await next();

        // Only process domain events if the request was successful
        if (response is Result<object> result && result.IsSuccess)
        {
            // Check if the response contains an aggregate root
            if (result.Value is IAggregateRoot aggregateRoot)
            {
                await PublishDomainEvents(aggregateRoot, cancellationToken);
            }
            // Check if the response is a collection of aggregate roots
            else if (result.Value is IEnumerable<IAggregateRoot> aggregateRoots)
            {
                foreach (var aggregate in aggregateRoots)
                {
                    await PublishDomainEvents(aggregate, cancellationToken);
                }
            }
        }

        return response;
    }

    /// <summary>
    /// Publishes all domain events from an aggregate root and clears them
    /// </summary>
    /// <param name="aggregateRoot">The aggregate root containing domain events</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task PublishDomainEvents(IAggregateRoot aggregateRoot, CancellationToken cancellationToken)
    {
        var events = aggregateRoot.DomainEvents.ToList();

        if (!events.Any())
        {
            return;
        }

        _logger.LogInformation("Publishing {EventCount} domain events for request {RequestName}",
            events.Count, typeof(TRequest).Name);

        // Publish each domain event
        foreach (var domainEvent in events)
        {
            _logger.LogDebug("Publishing domain event {EventName}", domainEvent.GetType().Name);
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        // Clear the events since they've been published
        aggregateRoot.ClearDomainEvents();
    }
}
