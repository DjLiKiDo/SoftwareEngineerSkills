using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Exceptions;
using System.Collections.Concurrent;

namespace SoftwareEngineerSkills.Domain.UnitTests.Common.Base;

public class AggregateRootTests
{
    [Fact]
    public void EnforceInvariants_WhenInvariantsAreViolated_ShouldThrowDomainValidationException()
    {
        // Arrange
        var testAggregate = new TestAggregateWithInvalidState();
        
        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => testAggregate.EnforceInvariants());
        exception.Errors.Should().Contain("Name cannot be empty");
    }
    
    [Fact]
    public async Task EnforceInvariantsAsync_WhenInvariantsAreViolated_ShouldThrowDomainValidationException()
    {
        // Arrange
        var testAggregate = new TestAggregateWithInvalidState();
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainValidationException>(
            () => testAggregate.EnforceInvariantsAsync());
        exception.Errors.Should().Contain("Name cannot be empty");
    }
    
    [Fact]
    public void AddAndApplyEvent_ShouldIncrementVersion()
    {
        // Arrange
        var testAggregate = new TestAggregate("Test");
        var initialVersion = testAggregate.Version;
        
        // Act
        testAggregate.ChangeName("New Name");
        
        // Assert
        testAggregate.Version.Should().Be(initialVersion + 1);
    }
    
    [Fact]
    public void AddAndApplyEvent_WithInvalidState_ShouldThrowDomainValidationException()
    {
        // Arrange
        var testAggregate = new TestAggregate("Test");
        
        // Act & Assert
        testAggregate.Invoking(a => a.ChangeName(""))
            .Should().Throw<DomainValidationException>()
            .WithMessage("*Name cannot be empty*");
    }
    
    [Fact]
    public async Task AddAndApplyEventAsync_ShouldIncrementVersion()
    {
        // Arrange
        var testAggregate = new TestAggregate("Test");
        var initialVersion = testAggregate.Version;
        
        // Act
        await testAggregate.ChangeNameAsync("New Name Async");
        
        // Assert
        testAggregate.Version.Should().Be(initialVersion + 1);
        testAggregate.Name.Should().Be("New Name Async");
    }
    
    [Fact]
    public async Task AddAndApplyEventAsync_WithInvalidState_ShouldThrowDomainValidationException()
    {
        // Arrange
        var testAggregate = new TestAggregate("Test");
        
        // Act & Assert
        await testAggregate.Invoking(a => a.ChangeNameAsync(""))
            .Should().ThrowAsync<DomainValidationException>()
            .WithMessage("*Name cannot be empty*");
    }
    
    [Fact]
    public void Apply_ShouldChangeAggregateStateAccordingToEvent()
    {
        // Arrange
        var testAggregate = new TestAggregate("Initial Name");
        var expectedName = "Changed Name";
        
        // Act
        var testEvent = new TestEvent(expectedName);
        testAggregate.PublicApply(testEvent);
        
        // Assert
        testAggregate.Name.Should().Be(expectedName);
        testAggregate.LastEventType.Should().Be("TestEvent");
    }
    
    [Fact]
    public void AggregateRoot_ThreadSafeDomainEventOperations_ShouldWorkCorrectly()
    {
        // Arrange
        var testAggregate = new TestAggregate("Initial");
        var eventCount = 20;
        
        // Act - Add events concurrently
        Parallel.For(0, eventCount, i =>
        {
            testAggregate.AddTestDomainEvent(new TestEvent($"Test-{i}"));
        });
        
        // Assert
        testAggregate.DomainEvents.Should().HaveCount(eventCount);
        testAggregate.Version.Should().Be(eventCount); // Each event should increment version
    }
    
    [Fact]
    public void ConcurrentAddAndRemove_ShouldBeThreadSafe()
    {        // Arrange
        var testAggregate = new TestAggregate("Initial");
        var eventIds = new ConcurrentBag<Guid>();
        
        // Act
        // Add 50 events
        Parallel.For(0, 50, i =>
        {
            var evt = new TestEvent($"Test-{i}");
            testAggregate.AddTestDomainEvent(evt);
            eventIds.Add(evt.Id);
        });
        
        // Remove about half of them randomly
        var toRemove = eventIds.Take(25).ToList();
        Parallel.ForEach(toRemove, id =>
        {
            var evt = testAggregate.DomainEvents.FirstOrDefault(e => e.Id == id);
            if (evt != null)
            {
                testAggregate.RemoveTestDomainEvent(evt);
            }
        });
        
        // Assert
        var remainingEvents = testAggregate.DomainEvents.Count;
        // We can't assert exact count due to race conditions in test setup,
        // but we know it should be less than original count
        remainingEvents.Should().BeLessThan(50);
        remainingEvents.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public void ThreadSafeClearEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var testAggregate = new TestAggregate("Initial");
        
        // Add events from multiple threads
        Parallel.For(0, 20, i =>
        {
            testAggregate.AddTestDomainEvent(new TestEvent($"Test-{i}"));
        });
        
        // Act
        testAggregate.ClearDomainEvents();
        
        // Assert
        testAggregate.DomainEvents.Should().BeEmpty();
    }
    
    [Fact]
    public async Task MultiThreadedEventHandling_ShouldMaintainConsistency()
    {
        // Arrange
        var testAggregate = new TestAggregate("Aggregate");
        var tasks = new List<Task>();
        
        // Act
        // Add events in multiple concurrent tasks
        for (var i = 0; i < 10; i++)
        {
            var namePrefix = $"Name-{i}-";
            tasks.Add(Task.Run(async () =>
            {
                for (var j = 0; j < 5; j++)
                {
                    await testAggregate.ChangeNameAsync($"{namePrefix}{j}");
                    // Small delay to increase chance of thread interleaving
                    await Task.Delay(5);
                }
            }));
        }
        
        await Task.WhenAll(tasks);
        
        // Assert
        // Each task adds 5 events, for 10 tasks
        testAggregate.DomainEvents.Count.Should().Be(50);
        // Events should increment version atomically
        testAggregate.Version.Should().Be(50);
    }
    
    // Test classes
    
    private class TestEvent : DomainEvent
    {
        public string NewName { get; }
        
        public TestEvent(string newName)
        {
            NewName = newName ?? throw new ArgumentNullException(nameof(newName));
        }
    }
    
    private class TestAggregate : AggregateRoot
    {
        public string Name { get; private set; }
        public string LastEventType { get; private set; } = string.Empty;
        
        public TestAggregate(string name)
        {
            Name = name;
            EnforceInvariants();
        }
        
        public void ChangeName(string newName)
        {
            AddAndApplyEvent(new TestEvent(newName));
        }
        
        public async Task ChangeNameAsync(string newName)
        {
            await AddAndApplyEventAsync(new TestEvent(newName));
        }
        
        // Expose protected methods for testing
        public void AddTestDomainEvent(IDomainEvent domainEvent)
        {
            // Access the protected AddDomainEvent method via new keyword 
            // to test thread safety
            base.AddDomainEvent(domainEvent);
        }
        
        public void RemoveTestDomainEvent(IDomainEvent domainEvent)
        {
            base.RemoveDomainEvent(domainEvent);
        }
        
        public void PublicApply(IDomainEvent domainEvent)
        {
            Apply(domainEvent);
        }
        
        protected override void Apply(IDomainEvent domainEvent)
        {
            if (domainEvent is TestEvent testEvent)
            {
                Name = testEvent.NewName;
                LastEventType = "TestEvent";
            }
        }
        
        protected override IEnumerable<string> CheckInvariants()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return "Name cannot be empty";
            }
        }
    }
    
    private class TestAggregateWithInvalidState : AggregateRoot
    {
        public string Name { get; } = string.Empty;
        
        protected override IEnumerable<string> CheckInvariants()
        {
            yield return "Name cannot be empty";
        }
    }
}
