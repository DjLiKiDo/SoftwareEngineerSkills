using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Exceptions;
using Xunit;

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
        Assert.Contains("Name cannot be empty", exception.Errors);
    }
    
    [Fact]
    public async Task EnforceInvariantsAsync_WhenInvariantsAreViolated_ShouldThrowDomainValidationException()
    {
        // Arrange
        var testAggregate = new TestAggregateWithInvalidState();
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainValidationException>(
            () => testAggregate.EnforceInvariantsAsync());
        Assert.Contains("Name cannot be empty", exception.Errors);
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
        Assert.Equal(initialVersion + 1, testAggregate.Version);
    }
    
    [Fact]
    public void AddAndApplyEvent_WithInvalidState_ShouldThrowDomainValidationException()
    {
        // Arrange
        var testAggregate = new TestAggregate("Test");
        
        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => testAggregate.ChangeName(""));
        Assert.Contains("Name cannot be empty", exception.Errors);
    }
    
    // Test classes
    
    private class TestEvent : DomainEvent
    {
        public string NewName { get; }
        
        public TestEvent(string newName)
        {
            NewName = newName;
        }
    }
    
    private class TestAggregate : AggregateRoot
    {
        public string Name { get; private set; }
        
        public TestAggregate(string name)
        {
            Name = name;
            EnforceInvariants();
        }
        
        public void ChangeName(string newName)
        {
            AddAndApplyEvent(new TestEvent(newName));
        }
        
        protected override void Apply(IDomainEvent domainEvent)
        {
            if (domainEvent is TestEvent testEvent)
            {
                Name = testEvent.NewName;
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
            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return "Name cannot be empty";
            }
        }
    }
}
