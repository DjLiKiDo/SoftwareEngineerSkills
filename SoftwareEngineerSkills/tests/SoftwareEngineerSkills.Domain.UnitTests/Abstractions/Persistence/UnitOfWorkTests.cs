using FluentAssertions;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Abstractions.Persistence;

/// <summary>
/// Unit tests for IUnitOfWork interface
/// </summary>
public class UnitOfWorkTests
{
    [Fact]
    public void IUnitOfWork_ShouldHaveRequiredMembers()
    {
        // Arrange
        var type = typeof(IUnitOfWork);
        
        // Act & Assert
        type.Should().BeInterface();
        
        // Check Skills property
        var skillsProperty = type.GetProperty("Skills");
        skillsProperty.Should().NotBeNull();
        skillsProperty.PropertyType.Should().Be(typeof(ISkillRepository));
        skillsProperty.CanRead.Should().BeTrue();
        skillsProperty.CanWrite.Should().BeFalse(); // Should only have a getter
        
        // Check SaveChangesAsync method
        var saveChangesMethod = type.GetMethod("SaveChangesAsync");
        saveChangesMethod.Should().NotBeNull();
        saveChangesMethod.ReturnType.Should().Be(typeof(Task<int>));
        saveChangesMethod.GetParameters().Should().HaveCount(1);
        saveChangesMethod.GetParameters()[0].ParameterType.Should().Be(typeof(CancellationToken));
    }

    [Fact]
    public void IUnitOfWork_ImplementsIDisposable()
    {
        // Arrange
        var type = typeof(IUnitOfWork);
        
        // Act & Assert
        type.GetInterface("IDisposable").Should().NotBeNull();
    }
}
