using FluentAssertions;
using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Common.Base;

public class AggregateRootAsyncTests
{
    [Fact]
    public async Task AddAndApplyEventAsync_ShouldApplyEventAndAddToCollection()
    {
        // Arrange
        var aggregate = new TestAggregateRoot("Initial Name");
        var initialVersion = aggregate.Version;

        // Act
        await aggregate.ChangeNameAsync("New Name");

        // Assert
        aggregate.Name.Should().Be("New Name");
        aggregate.Version.Should().Be(initialVersion + 1);
        aggregate.DomainEvents.Should().ContainSingle();
        var domainEvent = aggregate.DomainEvents.Single() as NameChangedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.NewName.Should().Be("New Name");
    }

    [Fact]
    public async Task AddAndApplyEventAsync_WhenMultipleEventsAreApplied_ShouldIncrementVersionCorrectly()
    {
        // Arrange
        var aggregate = new TestAggregateRoot("Initial Name");
        var initialVersion = aggregate.Version;

        // Act
        await aggregate.ChangeNameAsync("Name 1");
        await aggregate.ChangeNameAsync("Name 2");
        await aggregate.ChangeNameAsync("Name 3");

        // Assert
        aggregate.Name.Should().Be("Name 3");
        aggregate.Version.Should().Be(initialVersion + 3);
        aggregate.DomainEvents.Should().HaveCount(3);
    }

    [Fact]
    public async Task AddAndApplyEvent_WithThreadSafety_ShouldHandleMultipleThreadsCorrectly()
    {
        // Arrange
        var aggregate = new TestAggregateRoot("Initial Name");
        var initialVersion = aggregate.Version;
        var taskCount = 10;

        // Act
        var tasks = new List<Task>();
        for (int i = 0; i < taskCount; i++)
        {
            string newName = $"Name {i}";
            tasks.Add(Task.Run(() => aggregate.ChangeName(newName)));
        }
        await Task.WhenAll(tasks.ToArray());

        // Assert
        aggregate.Version.Should().Be(initialVersion + taskCount);
        aggregate.DomainEvents.Should().HaveCount(taskCount);
    }

    [Fact]
    public async Task AddAndApplyEventAsync_WithThreadSafety_ShouldHandleMultipleThreadsCorrectly()
    {
        // Arrange
        var aggregate = new TestAggregateRoot("Initial Name");
        var initialVersion = aggregate.Version;
        var taskCount = 10;

        // Act
        var tasks = new List<Task>();
        for (int i = 0; i < taskCount; i++)
        {
            string newName = $"Name {i}";
            tasks.Add(aggregate.ChangeNameAsync(newName));
        }
        await Task.WhenAll(tasks);

        // Assert
        aggregate.Version.Should().Be(initialVersion + taskCount);
        aggregate.DomainEvents.Should().HaveCount(taskCount);
    }

    #region Helper Classes
    
    public class NameChangedEvent : DomainEvent
    {
        public string NewName { get; }
        
        public NameChangedEvent(string newName)
        {
            NewName = newName;
        }
    }
    
    private class TestAggregateRoot : AggregateRoot
    {
        public string Name { get; private set; }
        
        public TestAggregateRoot(string name)
        {
            Name = name;
        }
        
        public void ChangeName(string newName)
        {
            AddAndApplyEvent(new NameChangedEvent(newName));
        }
        
        public async Task ChangeNameAsync(string newName)
        {
            await AddAndApplyEventAsync(new NameChangedEvent(newName));
        }
        
        protected override void Apply(IDomainEvent domainEvent)
        {
            if (domainEvent is NameChangedEvent nameChangedEvent)
            {
                Name = nameChangedEvent.NewName;
            }
        }
    }
    
    #endregion
}
