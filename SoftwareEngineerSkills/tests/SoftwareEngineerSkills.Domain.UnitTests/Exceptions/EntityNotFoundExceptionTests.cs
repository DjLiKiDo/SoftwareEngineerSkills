using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Domain.UnitTests.Exceptions;

public class EntityNotFoundExceptionTests
{
    [Fact]
    public void Constructor_WithIdAndType_ShouldCreateInstance()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entityType = typeof(TestEntity);

        // Act
        var exception = new EntityNotFoundException(id, entityType);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be($"Entity of type TestEntity with ID {id} was not found");
        exception.EntityId.Should().Be(id);
        exception.EntityType.Should().Be(entityType);
        exception.EntityTypeName.Should().Be("TestEntity");
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithIdAndTypeName_ShouldCreateInstance()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entityTypeName = "Customer";

        // Act
        var exception = new EntityNotFoundException(id, entityTypeName);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be($"Entity of type Customer with ID {id} was not found");
        exception.EntityId.Should().Be(id);
        exception.EntityType.Should().Be(typeof(object));
        exception.EntityTypeName.Should().Be("Customer");
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessage_ShouldCreateInstance()
    {
        // Arrange
        var message = "Custom not found message";

        // Act
        var exception = new EntityNotFoundException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.EntityId.Should().Be(Guid.Empty);
        exception.EntityType.Should().Be(typeof(object));
        exception.EntityTypeName.Should().Be("Unknown");
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldCreateInstance()
    {
        // Arrange
        var message = "Custom not found message";
        var innerException = new ArgumentException("Inner exception");

        // Act
        var exception = new EntityNotFoundException(message, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.EntityId.Should().Be(Guid.Empty);
        exception.EntityType.Should().Be(typeof(object));
        exception.EntityTypeName.Should().Be("Unknown");
        exception.InnerException.Should().BeEquivalentTo(innerException);
    }

    [Fact]
    public void EntityNotFoundException_ShouldInheritFromDomainException()
    {
        // Act
        var exception = new EntityNotFoundException("Test");

        // Assert
        exception.Should().BeAssignableTo<DomainException>();
    }

    // Helper class for testing
    private class TestEntity { }
}
