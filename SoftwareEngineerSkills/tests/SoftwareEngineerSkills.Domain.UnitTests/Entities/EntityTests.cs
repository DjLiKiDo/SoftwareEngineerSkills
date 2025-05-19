using FluentAssertions;
using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Entities;
using System;
using System.Linq;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Entities;

public class EntityTests
{
    [Fact]
    public void Equals_SameReference_ShouldReturnTrue()
    {
        // Arrange
        var entity = new TestEntity();

        // Act & Assert
        entity.Equals(entity).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentType_ShouldReturnFalse()
    {
        // Arrange
        var entity = new TestEntity();
        var differentEntity = new DifferentTestEntity();

        // Act & Assert
        entity.Equals(differentEntity).Should().BeFalse();
    }

    [Fact]
    public void Equals_NullEntity_ShouldReturnFalse()
    {
        // Arrange
        var entity = new TestEntity();

        // Act & Assert
        entity.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_SameIdDifferentType_ShouldReturnFalse()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var entity1 = new TestEntity(guid);
        var entity2 = new DifferentTestEntity(guid);

        // Act & Assert
        entity1.Equals(entity2).Should().BeFalse();
    }

    [Fact]
    public void Equals_SameIdSameType_ShouldReturnTrue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var entity1 = new TestEntity(guid);
        var entity2 = new TestEntity(guid);

        // Act & Assert
        entity1.Equals(entity2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentIdSameType_ShouldReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act & Assert
        entity1.Equals(entity2).Should().BeFalse();
    }

    [Fact]
    public void Equals_EmptyIdSameType_ShouldReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.Empty);
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act & Assert
        entity1.Equals(entity2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameId_ShouldReturnSameHashCode()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var entity1 = new TestEntity(guid);
        var entity2 = new TestEntity(guid);

        // Act
        var hashCode1 = entity1.GetHashCode();
        var hashCode2 = entity2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void GetHashCode_DifferentId_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act
        var hashCode1 = entity1.GetHashCode();
        var hashCode2 = entity2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }

    [Fact]
    public void EqualityOperator_SameEntity_ShouldReturnTrue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var entity1 = new TestEntity(guid);
        var entity2 = new TestEntity(guid);

        // Act & Assert
        (entity1 == entity2).Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_DifferentEntity_ShouldReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act & Assert
        (entity1 == entity2).Should().BeFalse();
    }

    [Fact]
    public void InequalityOperator_SameEntity_ShouldReturnFalse()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var entity1 = new TestEntity(guid);
        var entity2 = new TestEntity(guid);

        // Act & Assert
        (entity1 != entity2).Should().BeFalse();
    }

    [Fact]
    public void InequalityOperator_DifferentEntity_ShouldReturnTrue()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act & Assert
        (entity1 != entity2).Should().BeTrue();
    }
    
    [Fact]
    public void EqualityOperator_NullEntities_ShouldReturnTrue()
    {
        // Arrange
        TestEntity? entity1 = null;
        TestEntity? entity2 = null;

        // Act & Assert
        (entity1 == entity2).Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_OneNullEntity_ShouldReturnFalse()
    {
        // Arrange
        TestEntity? entity1 = new TestEntity();
        TestEntity? entity2 = null;

        // Act & Assert
        (entity1 == entity2).Should().BeFalse();
        (entity2 == entity1).Should().BeFalse();
    }

    #region Helper Classes
    
    private class TestEntity : Entity
    {
        public TestEntity() : base() { }
        
        public TestEntity(Guid id)
        {
            Id = id;
        }
    }
    
    private class DifferentTestEntity : Entity
    {
        public DifferentTestEntity() : base() { }
        
        public DifferentTestEntity(Guid id)
        {
            Id = id;
        }
    }
    
    #endregion
}
