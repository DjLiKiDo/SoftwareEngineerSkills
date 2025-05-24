using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Domain.UnitTests.Common.Base;

public class BaseEntityInvariantTests
{
    [Fact]
    public void EnforceInvariants_WhenNoInvariants_ShouldNotThrow()
    {
        // Arrange
        var entity = new TestEntityWithNoInvariants();

        // Act & Assert
        entity.Invoking(e => e.EnforceInvariants()).Should().NotThrow();
    }

    [Fact]
    public void EnforceInvariants_WhenInvariantsViolated_ShouldThrowDomainValidationException()
    {
        // Arrange
        var entity = new TestEntityWithInvariants();

        // Act & Assert
        entity.Invoking(e => e.EnforceInvariants())
            .Should().Throw<DomainValidationException>()
            .Which.Errors.Should().Contain("Test invariant violated");
    }

    [Fact]
    public void EnforceInvariants_WhenMultipleInvariantsViolated_ShouldThrowWithAllErrors()
    {
        // Arrange
        var entity = new TestEntityWithMultipleInvariants();

        // Act & Assert
        entity.Invoking(e => e.EnforceInvariants())
            .Should().Throw<DomainValidationException>()
            .Which.Errors.Should().BeEquivalentTo(new[] 
            { 
                "First invariant violated", 
                "Second invariant violated" 
            });
    }

    [Fact]
    public async Task EnforceInvariantsAsync_WhenNoInvariants_ShouldNotThrow()
    {
        // Arrange
        var entity = new TestEntityWithNoInvariants();

        // Act & Assert
        await entity.Invoking(e => e.EnforceInvariantsAsync()).Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnforceInvariantsAsync_WhenInvariantsViolated_ShouldThrowDomainValidationException()
    {
        // Arrange
        var entity = new TestEntityWithInvariants();

        // Act & Assert
        await entity.Invoking(e => e.EnforceInvariantsAsync())
            .Should().ThrowAsync<DomainValidationException>()
            .WithMessage("*One or more domain validation*");
    }

    [Fact]
    public async Task EnforceInvariantsAsync_WhenMultipleInvariantsViolated_ShouldThrowWithAllErrors()
    {
        // Arrange
        var entity = new TestEntityWithMultipleInvariants();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainValidationException>(
            async () => await entity.EnforceInvariantsAsync());
        exception.Errors.Should().BeEquivalentTo(new[] 
        { 
            "First invariant violated", 
            "Second invariant violated" 
        });
    }

    [Fact]
    public void IncrementVersion_ShouldIncrementTheVersionProperty()
    {
        // Arrange
        var entity = new TestEntityWithNoInvariants();
        var initialVersion = entity.Version;

        // Act
        entity.IncrementVersionPublic();

        // Assert
        entity.Version.Should().Be(initialVersion + 1);
    }

    #region Helper Classes

    private class TestEntityWithNoInvariants : BaseEntity
    {
        public void IncrementVersionPublic()
        {
            IncrementVersion();
        }
    }

    private class TestEntityWithInvariants : BaseEntity
    {
        protected override IEnumerable<string> CheckInvariants()
        {
            yield return "Test invariant violated";
        }
    }

    private class TestEntityWithMultipleInvariants : BaseEntity
    {
        protected override IEnumerable<string> CheckInvariants()
        {
            yield return "First invariant violated";
            yield return "Second invariant violated";
        }
    }

    #endregion
}
