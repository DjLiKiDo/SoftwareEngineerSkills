using FluentAssertions;
using Moq;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Common.Interfaces;

public class IAggregateRootTests
{
    [Fact]
    public void GetMembers_IAggregateRoot_ShouldHaveRequiredProperties()
    {
        // Arrange
        var type = typeof(IAggregateRoot);
        
        // Act & Assert
        type.Should().BeInterface();
        
        // Check the DomainEvents property
        var domainEventsProperty = type.GetProperty("DomainEvents");
        domainEventsProperty.Should().NotBeNull();
        domainEventsProperty.PropertyType.Should().Be(typeof(IReadOnlyCollection<IDomainEvent>));
        domainEventsProperty.CanRead.Should().BeTrue();
        domainEventsProperty.CanWrite.Should().BeFalse(); // Should only have a getter
        
        // Check the EnforceInvariantsAsync method
        var enforceInvariantsMethod = type.GetMethod("EnforceInvariantsAsync");
        enforceInvariantsMethod.Should().NotBeNull();
        enforceInvariantsMethod.ReturnType.Should().Be(typeof(Task));
    }
    
    [Fact]
    public void Implementation_IAggregateRoot_ShouldWorkAsExpected()
    {
        // Arrange
        var mockAggregateRoot = new Mock<IAggregateRoot>();
        var domainEvents = new List<IDomainEvent>();
        
        mockAggregateRoot.Setup(ar => ar.DomainEvents)
            .Returns(domainEvents.AsReadOnly());
        
        mockAggregateRoot.Setup(ar => ar.EnforceInvariantsAsync())
            .Returns(Task.CompletedTask);
        
        // Act
        var resultEvents = mockAggregateRoot.Object.DomainEvents;
        var enforceTask = mockAggregateRoot.Object.EnforceInvariantsAsync();
        
        // Assert
        resultEvents.Should().BeEmpty();
        enforceTask.IsCompleted.Should().BeTrue();
        mockAggregateRoot.Verify(ar => ar.DomainEvents, Times.Once());
        mockAggregateRoot.Verify(ar => ar.EnforceInvariantsAsync(), Times.Once());
    }
}

public class IAuditableEntityTests
{
    [Fact]
    public void GetMembers_IAuditableEntity_ShouldHaveRequiredProperties()
    {
        // Arrange
        var type = typeof(IAuditableEntity);
        
        // Act & Assert
        type.Should().BeInterface();
        
        // Check Created property
        var createdProperty = type.GetProperty("Created");
        createdProperty.Should().NotBeNull();
        createdProperty.PropertyType.Should().Be(typeof(DateTime));
        createdProperty.CanRead.Should().BeTrue();
        createdProperty.CanWrite.Should().BeTrue();
        
        // Check CreatedBy property
        var createdByProperty = type.GetProperty("CreatedBy");
        createdByProperty.Should().NotBeNull();
        createdByProperty.PropertyType.Should().Be(typeof(string).MakeNullable());
        createdByProperty.CanRead.Should().BeTrue();
        createdByProperty.CanWrite.Should().BeTrue();
        
        // Check LastModified property
        var lastModifiedProperty = type.GetProperty("LastModified");
        lastModifiedProperty.Should().NotBeNull();
        lastModifiedProperty.PropertyType.Should().Be(typeof(DateTime?));
        lastModifiedProperty.CanRead.Should().BeTrue();
        lastModifiedProperty.CanWrite.Should().BeTrue();
        
        // Check LastModifiedBy property
        var lastModifiedByProperty = type.GetProperty("LastModifiedBy");
        lastModifiedByProperty.Should().NotBeNull();
        lastModifiedByProperty.PropertyType.Should().Be(typeof(string).MakeNullable());
        lastModifiedByProperty.CanRead.Should().BeTrue();
        lastModifiedByProperty.CanWrite.Should().BeTrue();
    }
    
    [Fact]
    public void Implementation_IAuditableEntity_ShouldSetAndGetProperties()
    {
        // Arrange
        var mockAuditableEntity = new Mock<IAuditableEntity>();
        var now = DateTime.UtcNow;
        var creator = "TestUser";
        var modifier = "AnotherUser";
        
        mockAuditableEntity.SetupProperty(ae => ae.Created);
        mockAuditableEntity.SetupProperty(ae => ae.CreatedBy);
        mockAuditableEntity.SetupProperty(ae => ae.LastModified);
        mockAuditableEntity.SetupProperty(ae => ae.LastModifiedBy);
        
        // Act
        mockAuditableEntity.Object.Created = now;
        mockAuditableEntity.Object.CreatedBy = creator;
        mockAuditableEntity.Object.LastModified = now.AddDays(1);
        mockAuditableEntity.Object.LastModifiedBy = modifier;
        
        // Assert
        mockAuditableEntity.Object.Created.Should().Be(now);
        mockAuditableEntity.Object.CreatedBy.Should().Be(creator);
        mockAuditableEntity.Object.LastModified.Should().Be(now.AddDays(1));
        mockAuditableEntity.Object.LastModifiedBy.Should().Be(modifier);
    }
}

public class ISoftDeleteTests
{
    [Fact]
    public void GetMembers_ISoftDelete_ShouldHaveRequiredProperties()
    {
        // Arrange
        var type = typeof(ISoftDelete);
        
        // Act & Assert
        type.Should().BeInterface();
        
        // Check IsDeleted property
        var isDeletedProperty = type.GetProperty("IsDeleted");
        isDeletedProperty.Should().NotBeNull();
        isDeletedProperty.PropertyType.Should().Be(typeof(bool));
        isDeletedProperty.CanRead.Should().BeTrue();
        isDeletedProperty.CanWrite.Should().BeTrue();
        
        // Check DeletedAt property
        var deletedAtProperty = type.GetProperty("DeletedAt");
        if (deletedAtProperty == null) // Try alternate name
        {
            deletedAtProperty = type.GetProperty("DeletedAt");
        }
        deletedAtProperty.Should().NotBeNull();
        deletedAtProperty.PropertyType.Should().Be(typeof(DateTime?));
        deletedAtProperty.CanRead.Should().BeTrue();
        deletedAtProperty.CanWrite.Should().BeTrue();
        
        // Check DeletedBy property
        var deletedByProperty = type.GetProperty("DeletedBy");
        deletedByProperty.Should().NotBeNull();
        deletedByProperty.PropertyType.Should().Be(typeof(string).MakeNullable());
        deletedByProperty.CanRead.Should().BeTrue();
        deletedByProperty.CanWrite.Should().BeTrue();
    }
    
    [Fact]
    public void Implementation_ISoftDelete_ShouldSetAndGetProperties()
    {
        // Arrange
        var mockSoftDelete = new Mock<ISoftDelete>();
        var now = DateTime.UtcNow;
        var deleter = "DeleteUser";
        
        mockSoftDelete.SetupProperty(sd => sd.IsDeleted);
        mockSoftDelete.SetupProperty(sd => sd.DeletedAt);
        mockSoftDelete.SetupProperty(sd => sd.DeletedBy);
        
        // Act
        mockSoftDelete.Object.IsDeleted = true;
        mockSoftDelete.Object.DeletedAt = now;
        mockSoftDelete.Object.DeletedBy = deleter;
        
        // Assert
        mockSoftDelete.Object.IsDeleted.Should().BeTrue();
        mockSoftDelete.Object.DeletedAt.Should().Be(now);
        mockSoftDelete.Object.DeletedBy.Should().Be(deleter);
    }
}
