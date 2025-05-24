using FluentAssertions;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common.Base;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Abstractions.Persistence;

/// <summary>
/// Unit tests for repository interfaces
/// </summary>
public class RepositoryTests
{
    #region IRepository<T> Tests

    [Fact]
    public void IRepository_ShouldHaveRequiredMethods()
    {
        // Arrange
        var type = typeof(IRepository<TestEntity>);
        
        // Act & Assert
        type.IsInterface.Should().BeTrue();

        // Check AddAsync method
        var addMethod = type.GetMethod("AddAsync");
        addMethod.Should().NotBeNull();
        addMethod.ReturnType.Should().Be(typeof(Task));
        addMethod.GetParameters().Should().HaveCount(2);
        addMethod.GetParameters()[0].ParameterType.Should().Be(typeof(TestEntity));
        addMethod.GetParameters()[1].ParameterType.Should().Be(typeof(CancellationToken));
        
        // Check UpdateAsync method
        var updateMethod = type.GetMethod("UpdateAsync");
        updateMethod.Should().NotBeNull();
        updateMethod.ReturnType.Should().Be(typeof(Task));
        updateMethod.GetParameters().Should().HaveCount(2);
        updateMethod.GetParameters()[0].ParameterType.Should().Be(typeof(TestEntity));
        updateMethod.GetParameters()[1].ParameterType.Should().Be(typeof(CancellationToken));
        
        // Check DeleteAsync method
        var deleteMethod = type.GetMethod("DeleteAsync");
        deleteMethod.Should().NotBeNull();
        deleteMethod.ReturnType.Should().Be(typeof(Task));
        deleteMethod.GetParameters().Should().HaveCount(2);
        deleteMethod.GetParameters()[0].ParameterType.Should().Be(typeof(Guid));
        deleteMethod.GetParameters()[1].ParameterType.Should().Be(typeof(CancellationToken));
    }

    #endregion
    
    #region IReadRepository<T> Tests

    [Fact]
    public void IReadRepository_ShouldHaveRequiredMethods()
    {
        // Arrange
        var type = typeof(IReadRepository<TestEntity>);
        
        // Act & Assert
        type.IsInterface.Should().BeTrue();

        // Check GetByIdAsync method
        var getByIdMethod = type.GetMethod("GetByIdAsync");
        getByIdMethod.Should().NotBeNull();
        getByIdMethod.ReturnType.Should().Be(typeof(Task<TestEntity>));
        getByIdMethod.GetParameters().Should().HaveCount(2);
        getByIdMethod.GetParameters()[0].ParameterType.Should().Be(typeof(Guid));
        getByIdMethod.GetParameters()[1].ParameterType.Should().Be(typeof(CancellationToken));
        
        // Check ListAllAsync method
        var listAllMethod = type.GetMethod("ListAllAsync");
        listAllMethod.Should().NotBeNull();
        listAllMethod.ReturnType.Should().Be(typeof(Task<IReadOnlyList<TestEntity>>));
        listAllMethod.GetParameters().Should().HaveCount(1);
        listAllMethod.GetParameters()[0].ParameterType.Should().Be(typeof(CancellationToken));
    }

    #endregion
    
    #region ISoftDeleteRepository<T> Tests

    [Fact]
    public void ISoftDeleteRepository_ShouldHaveRequiredMethods()
    {
        // Arrange
        var type = typeof(ISoftDeleteRepository<TestEntity>);
        
        // Act & Assert
        type.IsInterface.Should().BeTrue();

        // Check SoftDeleteAsync method
        var softDeleteMethod = type.GetMethod("SoftDeleteAsync");
        softDeleteMethod.Should().NotBeNull();
        softDeleteMethod.ReturnType.Should().Be(typeof(Task));
        softDeleteMethod.GetParameters().Should().HaveCount(3);
        softDeleteMethod.GetParameters()[0].ParameterType.Should().Be(typeof(Guid));
        softDeleteMethod.GetParameters()[1].ParameterType.Should().Be(typeof(string));
        softDeleteMethod.GetParameters()[2].ParameterType.Should().Be(typeof(CancellationToken));
        
        // Check RestoreAsync method
        var restoreMethod = type.GetMethod("RestoreAsync");
        restoreMethod.Should().NotBeNull();
        restoreMethod.ReturnType.Should().Be(typeof(Task));
        restoreMethod.GetParameters().Should().HaveCount(3);
        restoreMethod.GetParameters()[0].ParameterType.Should().Be(typeof(Guid));
        restoreMethod.GetParameters()[1].ParameterType.Should().Be(typeof(string));
        restoreMethod.GetParameters()[2].ParameterType.Should().Be(typeof(CancellationToken));
        
        // Should inherit from IRepository<T>
        type.GetInterface(typeof(IRepository<TestEntity>).Name).Should().NotBeNull();
    }

    #endregion

    #region ISkillRepository Tests

    [Fact]
    public void ISkillRepository_ShouldHaveRequiredMethods()
    {
        // Arrange
        var type = typeof(ISkillRepository);
        
        // Act & Assert
        type.IsInterface.Should().BeTrue();

        // Should inherit from ISoftDeleteRepository<Skill>
        type.GetInterface(typeof(ISoftDeleteRepository<>).Name).Should().NotBeNull();
        
        // Check GetByNameAsync method
        var getByNameMethod = type.GetMethod("GetByNameAsync");
        getByNameMethod.Should().NotBeNull();
        getByNameMethod.ReturnType.Should().Be(typeof(Task<Domain.Entities.Skills.Skill>));
        getByNameMethod.GetParameters().Should().HaveCount(2);
        getByNameMethod.GetParameters()[0].ParameterType.Should().Be(typeof(string));
        getByNameMethod.GetParameters()[1].ParameterType.Should().Be(typeof(CancellationToken));
        
        // Check GetByCategoryAsync method
        var getByCategoryMethod = type.GetMethod("GetByCategoryAsync");
        getByCategoryMethod.Should().NotBeNull();
        getByCategoryMethod.ReturnType.Should().Be(typeof(Task<IReadOnlyList<Domain.Entities.Skills.Skill>>));
        getByCategoryMethod.GetParameters().Should().HaveCount(2);
        getByCategoryMethod.GetParameters()[0].ParameterType.Should().Be(typeof(Domain.Enums.SkillCategory));
        getByCategoryMethod.GetParameters()[1].ParameterType.Should().Be(typeof(CancellationToken));
    }

    #endregion

    #region TestEntity

    private class TestEntity : BaseEntity, SoftwareEngineerSkills.Domain.Common.Interfaces.ISoftDelete
    {
        public string Name { get; private set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        
        protected TestEntity() { }
        
        public TestEntity(string name)
        {
            Name = name;
        }
    }

    #endregion
}
