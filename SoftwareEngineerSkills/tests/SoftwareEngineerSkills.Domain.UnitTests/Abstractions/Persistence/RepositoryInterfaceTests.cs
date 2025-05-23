using FluentAssertions;
using Moq;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Abstractions.Persistence;

// Simple mock entity for testing
public class TestRepositoryEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
}

public class RepositoryInterfaceTests
{
    [Fact]
    public void IReadRepository_ShouldHaveExpectedMembers()
    {
        // Arrange
        var type = typeof(IReadRepository<>);
        
        // Act & Assert
        type.Should().BeInterface();
        type.IsGenericType.Should().BeTrue();
        
        // Check methods
        var methods = type.GetMethods();
        methods.Should().Contain(m => m.Name == "GetByIdAsync");
        methods.Should().Contain(m => m.Name == "GetByIdOrThrowAsync");
        methods.Should().Contain(m => m.Name == "GetAllAsync");
        methods.Should().Contain(m => m.Name == "FindAsync");
        methods.Should().Contain(m => m.Name == "AnyAsync");
        methods.Should().Contain(m => m.Name == "CountAsync");
        
        // Check GetByIdAsync specifically
        var getByIdMethod = methods.First(m => m.Name == "GetByIdAsync");
        getByIdMethod.ReturnType.Should().Be(typeof(Task<>).MakeGenericType(typeof(object).MakeNullable())); // This is approximate as the actual entity type is generic
    }
    
    [Fact]
    public void IRepository_ShouldExtendIReadRepository()
    {
        // Arrange
        var repoType = typeof(IRepository<>);
        var readRepoType = typeof(IReadRepository<>);
        
        // Act & Assert
        repoType.Should().BeInterface();
        repoType.IsGenericType.Should().BeTrue();
        
        // Due to generic types, we need to check interfaces differently
        repoType.GetInterfaces().Any(i => 
            i.IsGenericType && 
            i.GetGenericTypeDefinition() == readRepoType.GetGenericTypeDefinition()
        ).Should().BeTrue();
        
        // Check methods
        var methods = repoType.GetMethods();
        methods.Should().Contain(m => m.Name == "AddAsync");
        methods.Should().Contain(m => m.Name == "AddRangeAsync");
        methods.Should().Contain(m => m.Name == "Update");
        methods.Should().Contain(m => m.Name == "Remove");
        methods.Should().Contain(m => m.Name == "RemoveRange");
    }
    
    [Fact]
    public async Task IReadRepository_MockImplementation_ReturnsExpectedResults()
    {
        // Arrange
        var mockRepo = new Mock<IReadRepository<TestRepositoryEntity>>();
        var entity = new TestRepositoryEntity { Id = Guid.NewGuid(), Name = "Test" };
        var entities = new[] { entity };
        
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        mockRepo.Setup(r => r.GetByIdOrThrowAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        mockRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<TestRepositoryEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        mockRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<TestRepositoryEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        mockRepo.Setup(r => r.CountAsync(It.IsAny<Expression<Func<TestRepositoryEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        // Act & Assert
        var byId = await mockRepo.Object.GetByIdAsync(entity.Id);
        byId.Should().Be(entity);
        
        var byIdOrThrow = await mockRepo.Object.GetByIdOrThrowAsync(entity.Id);
        byIdOrThrow.Should().Be(entity);
        
        var all = await mockRepo.Object.GetAllAsync();
        all.Should().BeEquivalentTo(entities);
        
        var found = await mockRepo.Object.FindAsync(e => e.Name == "Test");
        found.Should().BeEquivalentTo(entities);
        
        var any = await mockRepo.Object.AnyAsync(e => e.Name == "Test");
        any.Should().BeTrue();
        
        var count = await mockRepo.Object.CountAsync(e => e.Name == "Test");
        count.Should().Be(1);
        
        // Verify method calls
        mockRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once());
        mockRepo.Verify(r => r.GetByIdOrThrowAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once());
        mockRepo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        mockRepo.Verify(r => r.FindAsync(It.IsAny<Expression<Func<TestRepositoryEntity, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
        mockRepo.Verify(r => r.AnyAsync(It.IsAny<Expression<Func<TestRepositoryEntity, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
        mockRepo.Verify(r => r.CountAsync(It.IsAny<Expression<Func<TestRepositoryEntity, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
    }
    
    [Fact]
    public async Task IRepository_MockImplementation_ExecutesAllMethods()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<TestRepositoryEntity>>();
        var entity = new TestRepositoryEntity { Id = Guid.NewGuid(), Name = "Test" };
        var entities = new[] { entity };
        
        mockRepo.Setup(r => r.AddAsync(It.IsAny<TestRepositoryEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockRepo.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<TestRepositoryEntity>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockRepo.Setup(r => r.Update(It.IsAny<TestRepositoryEntity>()));
        mockRepo.Setup(r => r.Remove(It.IsAny<TestRepositoryEntity>()));
        mockRepo.Setup(r => r.RemoveRange(It.IsAny<IEnumerable<TestRepositoryEntity>>()));
        
        // Act
        await mockRepo.Object.AddAsync(entity);
        await mockRepo.Object.AddRangeAsync(entities);
        mockRepo.Object.Update(entity);
        mockRepo.Object.Remove(entity);
        mockRepo.Object.RemoveRange(entities);
        
        // Assert
        mockRepo.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once());
        mockRepo.Verify(r => r.AddRangeAsync(entities, It.IsAny<CancellationToken>()), Times.Once());
        mockRepo.Verify(r => r.Update(entity), Times.Once());
        mockRepo.Verify(r => r.Remove(entity), Times.Once());
        mockRepo.Verify(r => r.RemoveRange(entities), Times.Once());
    }
}
