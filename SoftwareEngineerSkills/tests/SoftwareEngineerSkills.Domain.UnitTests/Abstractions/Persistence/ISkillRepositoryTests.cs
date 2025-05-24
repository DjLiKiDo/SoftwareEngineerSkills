using FluentAssertions;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Entities.Skills;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Abstractions.Persistence;

/// <summary>
/// Unit tests for ISkillRepository interface
/// </summary>
public class ISkillRepositoryTests
{
    [Fact]
    public void ISkillRepository_ShouldExtendIRepository()
    {
        // Arrange
        var type = typeof(ISkillRepository);
        
        // Act & Assert
        type.IsInterface.Should().BeTrue();
        
        // Check that it extends IRepository<Skill>
        type.GetInterfaces().Should().Contain(t => 
            t.IsGenericType && 
            t.GetGenericTypeDefinition() == typeof(IRepository<>) &&
            t.GetGenericArguments()[0] == typeof(Skill));
    }
    
    [Fact]
    public void ISkillRepository_ShouldHaveGetByNameAsyncMethod()
    {
        // Arrange
        var type = typeof(ISkillRepository);
        
        // Act & Assert
        var getByNameAsyncMethod = type.GetMethod("GetByNameAsync");
        getByNameAsyncMethod.Should().NotBeNull();
        
        var parameters = getByNameAsyncMethod!.GetParameters();
        parameters.Should().HaveCount(2);
        parameters[0].ParameterType.Should().Be(typeof(string));
        parameters[1].ParameterType.Should().Be(typeof(CancellationToken));
        parameters[1].IsOptional.Should().BeTrue();
        
        getByNameAsyncMethod.ReturnType.Should().Be(typeof(Task<Skill?>));
    }
    
    [Fact]
    public void ISkillRepository_ShouldHaveGetByCategoryAsyncMethod()
    {
        // Arrange
        var type = typeof(ISkillRepository);
        
        // Act & Assert
        var getByCategoryAsyncMethod = type.GetMethod("GetByCategoryAsync");
        getByCategoryAsyncMethod.Should().NotBeNull();
        
        var parameters = getByCategoryAsyncMethod!.GetParameters();
        parameters.Should().HaveCount(2);
        parameters[0].ParameterType.Should().Be(typeof(string));
        parameters[1].ParameterType.Should().Be(typeof(CancellationToken));
        parameters[1].IsOptional.Should().BeTrue();
        
        getByCategoryAsyncMethod.ReturnType.Should().Be(typeof(Task<IEnumerable<Skill>>));
    }
    
    [Fact]
    public void ISkillRepository_ShouldHaveGetByDifficultyLevelAsyncMethod()
    {
        // Arrange
        var type = typeof(ISkillRepository);
        
        // Act & Assert
        var getByDifficultyLevelAsyncMethod = type.GetMethod("GetByDifficultyLevelAsync");
        getByDifficultyLevelAsyncMethod.Should().NotBeNull();
        
        var parameters = getByDifficultyLevelAsyncMethod!.GetParameters();
        parameters.Should().HaveCount(2);
        parameters[0].ParameterType.Should().Be(typeof(int));
        parameters[1].ParameterType.Should().Be(typeof(CancellationToken));
        parameters[1].IsOptional.Should().BeTrue();
        
        getByDifficultyLevelAsyncMethod.ReturnType.Should().Be(typeof(Task<IEnumerable<Skill>>));
    }
    
    [Fact]
    public void ISkillRepository_ShouldHaveGetInDemandSkillsAsyncMethod()
    {
        // Arrange
        var type = typeof(ISkillRepository);
        
        // Act & Assert
        var getInDemandSkillsAsyncMethod = type.GetMethod("GetInDemandSkillsAsync");
        getInDemandSkillsAsyncMethod.Should().NotBeNull();
        
        var parameters = getInDemandSkillsAsyncMethod!.GetParameters();
        parameters.Should().HaveCount(1);
        parameters[0].ParameterType.Should().Be(typeof(CancellationToken));
        parameters[0].IsOptional.Should().BeTrue();
        
        getInDemandSkillsAsyncMethod.ReturnType.Should().Be(typeof(Task<IEnumerable<Skill>>));
    }
    
    [Fact]
    public void ISkillRepository_ShouldHaveExistsByNameAsyncMethod()
    {
        // Arrange
        var type = typeof(ISkillRepository);
        
        // Act & Assert
        var existsByNameAsyncMethod = type.GetMethod("ExistsByNameAsync");
        existsByNameAsyncMethod.Should().NotBeNull();
        
        var parameters = existsByNameAsyncMethod!.GetParameters();
        parameters.Should().HaveCount(2);
        parameters[0].ParameterType.Should().Be(typeof(string));
        parameters[1].ParameterType.Should().Be(typeof(CancellationToken));
        parameters[1].IsOptional.Should().BeTrue();
        
        existsByNameAsyncMethod.ReturnType.Should().Be(typeof(Task<bool>));
    }
}
