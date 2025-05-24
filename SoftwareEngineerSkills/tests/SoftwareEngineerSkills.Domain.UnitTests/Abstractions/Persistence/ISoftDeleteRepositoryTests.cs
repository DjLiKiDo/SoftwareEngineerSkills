using FluentAssertions;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Abstractions.Persistence;

/// <summary>
/// Unit tests for ISoftDeleteRepository interface
/// </summary>
public class ISoftDeleteRepositoryTests
{
    [Fact]
    public void ISoftDeleteRepository_ShouldExtendIRepository()
    {
        // Arrange
        var type = typeof(ISoftDeleteRepository<>);
        
        // Act & Assert
        type.IsGenericType.Should().BeTrue();
        type.IsInterface.Should().BeTrue();
        
        // Check that it extends IRepository
        type.GetInterfaces().Should().Contain(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IRepository<>));
    }
    
    [Fact]
    public void ISoftDeleteRepository_ShouldHaveGenericConstraints()
    {
        // Arrange
        var type = typeof(ISoftDeleteRepository<>);
        
        // Act
        var genericArg = type.GetGenericArguments()[0];
        
        // Assert
        genericArg.GetGenericParameterConstraints().Should().Contain(t => t == typeof(ISoftDelete));
        genericArg.GenericParameterAttributes.HasFlag(System.Reflection.GenericParameterAttributes.ReferenceTypeConstraint)
            .Should().BeTrue();
    }
    
    [Fact]
    public void ISoftDeleteRepository_ShouldHaveSoftDeleteMethod()
    {
        // Arrange
        var type = typeof(ISoftDeleteRepository<>);
        
        // Act & Assert
        var softDeleteMethod = type.GetMethod("SoftDelete");
        softDeleteMethod.Should().NotBeNull();
        
        var parameters = softDeleteMethod!.GetParameters();
        parameters.Should().HaveCount(2);
        parameters[0].ParameterType.Should().Be(type.GetGenericArguments()[0]);
        parameters[1].ParameterType.Should().Be(typeof(string).MakeByRefType().GetElementType()!.MakeNullableType());
        parameters[1].IsOptional.Should().BeTrue();
    }
    
    [Fact]
    public void ISoftDeleteRepository_ShouldHaveSoftDeleteRangeMethod()
    {
        // Arrange
        var type = typeof(ISoftDeleteRepository<>);
        
        // Act & Assert
        var softDeleteRangeMethod = type.GetMethod("SoftDeleteRange");
        softDeleteRangeMethod.Should().NotBeNull();
        
        var parameters = softDeleteRangeMethod!.GetParameters();
        parameters.Should().HaveCount(2);
        
        var entityType = type.GetGenericArguments()[0];
        parameters[0].ParameterType.Should().Be(typeof(IEnumerable<>).MakeGenericType(entityType));
        parameters[1].ParameterType.Should().Be(typeof(string).MakeByRefType().GetElementType()!.MakeNullableType());
        parameters[1].IsOptional.Should().BeTrue();
    }
    
    [Fact]
    public void ISoftDeleteRepository_ShouldHaveRestoreMethod()
    {
        // Arrange
        var type = typeof(ISoftDeleteRepository<>);
        
        // Act & Assert
        var restoreMethod = type.GetMethod("Restore");
        restoreMethod.Should().NotBeNull();
        
        var parameters = restoreMethod!.GetParameters();
        parameters.Should().HaveCount(1);
        parameters[0].ParameterType.Should().Be(type.GetGenericArguments()[0]);
    }
    
    [Fact]
    public void ISoftDeleteRepository_ShouldHaveGetSoftDeletedAsyncMethod()
    {
        // Arrange
        var type = typeof(ISoftDeleteRepository<>);
        
        // Act & Assert
        var getSoftDeletedMethod = type.GetMethod("GetSoftDeletedAsync");
        getSoftDeletedMethod.Should().NotBeNull();
        
        var parameters = getSoftDeletedMethod!.GetParameters();
        parameters.Should().HaveCount(1);
        parameters[0].ParameterType.Should().Be(typeof(CancellationToken));
        parameters[0].IsOptional.Should().BeTrue();
        
        var entityType = type.GetGenericArguments()[0];
        getSoftDeletedMethod.ReturnType.Should().Be(typeof(Task<>).MakeGenericType(typeof(IEnumerable<>).MakeGenericType(entityType)));
    }
    
    [Fact]
    public void ISoftDeleteRepository_ShouldHaveOverloadedGetByIdAsyncMethod()
    {
        // Arrange
        var type = typeof(ISoftDeleteRepository<>);
        
        // Act & Assert
        var getByIdAsyncMethods = type.GetMethods();
        var getByIdAsyncMethod = Array.Find(getByIdAsyncMethods, m => m.Name == "GetByIdAsync" && m.GetParameters().Length == 3);
        
        getByIdAsyncMethod.Should().NotBeNull();
        
        var parameters = getByIdAsyncMethod!.GetParameters();
        parameters.Should().HaveCount(3);
        parameters[0].ParameterType.Should().Be(typeof(Guid));
        parameters[1].ParameterType.Should().Be(typeof(bool));
        parameters[2].ParameterType.Should().Be(typeof(CancellationToken));
        parameters[2].IsOptional.Should().BeTrue();
        
        var entityType = type.GetGenericArguments()[0];
        var nullableEntityType = entityType.MakeNullableType();
        getByIdAsyncMethod.ReturnType.Should().Be(typeof(Task<>).MakeGenericType(nullableEntityType));
    }
    
    [Fact]
    public void ISoftDeleteRepository_ShouldHaveGetByIdOrThrowAsyncMethod()
    {
        // Arrange
        var type = typeof(ISoftDeleteRepository<>);
        
        // Act & Assert
        var getByIdOrThrowAsyncMethod = type.GetMethod("GetByIdOrThrowAsync");
        getByIdOrThrowAsyncMethod.Should().NotBeNull();
        
        var parameters = getByIdOrThrowAsyncMethod!.GetParameters();
        parameters.Should().HaveCount(3);
        parameters[0].ParameterType.Should().Be(typeof(Guid));
        parameters[1].ParameterType.Should().Be(typeof(bool));
        parameters[2].ParameterType.Should().Be(typeof(CancellationToken));
        parameters[2].IsOptional.Should().BeTrue();
        
        var entityType = type.GetGenericArguments()[0];
        getByIdOrThrowAsyncMethod.ReturnType.Should().Be(typeof(Task<>).MakeGenericType(entityType));
    }
}

public static class TypeExtensions
{
    public static Type MakeNullableType(this Type type)
    {
        if (type.IsValueType)
        {
            return typeof(Nullable<>).MakeGenericType(type);
        }
        
        return type;
    }
}
