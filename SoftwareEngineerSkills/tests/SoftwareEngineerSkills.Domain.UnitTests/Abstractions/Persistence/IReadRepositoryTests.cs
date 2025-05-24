using FluentAssertions;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Abstractions.Persistence;

/// <summary>
/// Unit tests for IReadRepository interface
/// </summary>
public class IReadRepositoryTests
{
    [Fact]
    public void IReadRepository_ShouldHaveGenericTypeParameter()
    {
        // Arrange
        var type = typeof(IReadRepository<>);
        
        // Act & Assert
        type.IsGenericType.Should().BeTrue();
        type.IsInterface.Should().BeTrue();
        type.GetGenericArguments().Should().HaveCount(1);
        
        // Check that it has a class constraint
        var genericArg = type.GetGenericArguments()[0];
        genericArg.GenericParameterAttributes.HasFlag(System.Reflection.GenericParameterAttributes.ReferenceTypeConstraint)
            .Should().BeTrue();
    }
    
    [Fact]
    public void IReadRepository_ShouldHaveGetByIdAsyncMethod()
    {
        // Arrange
        var type = typeof(IReadRepository<TestEntity>);
        
        // Act & Assert
        var getByIdAsyncMethod = type.GetMethod("GetByIdAsync");
        getByIdAsyncMethod.Should().NotBeNull();
        
        var parameters = getByIdAsyncMethod!.GetParameters();
        parameters.Should().HaveCount(2);
        parameters[0].ParameterType.Should().Be(typeof(Guid));
        parameters[1].ParameterType.Should().Be(typeof(CancellationToken));
        parameters[1].IsOptional.Should().BeTrue();
        
        getByIdAsyncMethod.ReturnType.Should().Be(typeof(Task<TestEntity?>));
    }
    
    [Fact]
    public void IReadRepository_ShouldHaveGetByIdOrThrowAsyncMethod()
    {
        // Arrange
        var type = typeof(IReadRepository<TestEntity>);
        
        // Act & Assert
        var getByIdOrThrowAsyncMethod = type.GetMethod("GetByIdOrThrowAsync");
        getByIdOrThrowAsyncMethod.Should().NotBeNull();
        
        var parameters = getByIdOrThrowAsyncMethod!.GetParameters();
        parameters.Should().HaveCount(2);
        parameters[0].ParameterType.Should().Be(typeof(Guid));
        parameters[1].ParameterType.Should().Be(typeof(CancellationToken));
        parameters[1].IsOptional.Should().BeTrue();
        
        getByIdOrThrowAsyncMethod.ReturnType.Should().Be(typeof(Task<TestEntity>));
    }
    
    [Fact]
    public void IReadRepository_ShouldHaveGetAllAsyncMethod()
    {
        // Arrange
        var type = typeof(IReadRepository<TestEntity>);
        
        // Act & Assert
        var getAllAsyncMethod = type.GetMethod("GetAllAsync");
        getAllAsyncMethod.Should().NotBeNull();
        
        var parameters = getAllAsyncMethod!.GetParameters();
        parameters.Should().HaveCount(1);
        parameters[0].ParameterType.Should().Be(typeof(CancellationToken));
        parameters[0].IsOptional.Should().BeTrue();
        
        getAllAsyncMethod.ReturnType.Should().Be(typeof(Task<IEnumerable<TestEntity>>));
    }
    
    [Fact]
    public void IReadRepository_ShouldHaveFindAsyncMethod()
    {
        // Arrange
        var type = typeof(IReadRepository<TestEntity>);
        
        // Act & Assert
        var findAsyncMethod = type.GetMethod("FindAsync");
        findAsyncMethod.Should().NotBeNull();
        
        var parameters = findAsyncMethod!.GetParameters();
        parameters.Should().HaveCount(2);
        parameters[0].ParameterType.Should().Be(typeof(Expression<Func<TestEntity, bool>>));
        parameters[1].ParameterType.Should().Be(typeof(CancellationToken));
        parameters[1].IsOptional.Should().BeTrue();
        
        findAsyncMethod.ReturnType.Should().Be(typeof(Task<IEnumerable<TestEntity>>));
    }
    
    [Fact]
    public void IReadRepository_ShouldHaveAnyAsyncMethod()
    {
        // Arrange
        var type = typeof(IReadRepository<TestEntity>);
        
        // Act & Assert
        var anyAsyncMethod = type.GetMethod("AnyAsync");
        anyAsyncMethod.Should().NotBeNull();
        
        var parameters = anyAsyncMethod!.GetParameters();
        parameters.Should().HaveCount(2);
        parameters[0].ParameterType.Should().Be(typeof(Expression<Func<TestEntity, bool>>));
        parameters[1].ParameterType.Should().Be(typeof(CancellationToken));
        parameters[1].IsOptional.Should().BeTrue();
        
        anyAsyncMethod.ReturnType.Should().Be(typeof(Task<bool>));
    }
    
    [Fact]
    public void IReadRepository_ShouldHaveCountAsyncMethod()
    {
        // Arrange
        var type = typeof(IReadRepository<TestEntity>);
        
        // Act & Assert
        var countAsyncMethod = type.GetMethod("CountAsync");
        countAsyncMethod.Should().NotBeNull();
        
        var parameters = countAsyncMethod!.GetParameters();
        parameters.Should().HaveCount(2);
        parameters[0].ParameterType.Should().Be(typeof(Expression<Func<TestEntity, bool>>));
        parameters[1].ParameterType.Should().Be(typeof(CancellationToken));
        parameters[1].IsOptional.Should().BeTrue();
        
        countAsyncMethod.ReturnType.Should().Be(typeof(Task<int>));
    }
}

/// <summary>
/// Test entity for repository tests
/// </summary>
public class TestEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}
