using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SoftwareEngineerSkills.Domain.Exceptions;
using SoftwareEngineerSkills.Infrastructure.Persistence;
using SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;
using SoftwareEngineerSkills.Infrastructure.UnitTests.TestHelpers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoftwareEngineerSkills.Infrastructure.UnitTests.Persistence.Repositories;

public class RepositoryGetByIdOrThrowTests
{
    [Fact]
    public async Task EfRepository_GetByIdOrThrowAsync_ExistingEntity_ShouldReturnEntity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
        
        using (var context = new ApplicationDbContext(options, Mock.Of<ICurrentUserAccessor>()))
        {
            await context.Set<TestEntity>().AddAsync(entity);
            await context.SaveChangesAsync();
        }
        
        using (var context = new ApplicationDbContext(options, Mock.Of<ICurrentUserAccessor>()))
        {
            var repository = new EfRepository<TestEntity>(context);
            
            // Act
            var result = await repository.GetByIdOrThrowAsync(entity.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(entity.Id);
            result.Name.Should().Be("Test Entity");
        }
    }
    
    [Fact]
    public async Task EfRepository_GetByIdOrThrowAsync_NonExistingEntity_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
            
        var nonExistingId = Guid.NewGuid();
        
        using (var context = new ApplicationDbContext(options, Mock.Of<ICurrentUserAccessor>()))
        {
            var repository = new EfRepository<TestEntity>(context);
            
            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => 
                repository.GetByIdOrThrowAsync(nonExistingId));
        }
    }
    
    [Fact]
    public async Task EfSoftDeleteRepository_GetByIdOrThrowAsync_ExistingNonDeletedEntity_ShouldReturnEntity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        
        var entity = new SoftDeleteTestEntity { Id = Guid.NewGuid(), Name = "Test Entity", IsDeleted = false };
        
        using (var context = new ApplicationDbContext(options, Mock.Of<ICurrentUserAccessor>()))
        {
            await context.Set<SoftDeleteTestEntity>().AddAsync(entity);
            await context.SaveChangesAsync();
        }
        
        using (var context = new ApplicationDbContext(options, Mock.Of<ICurrentUserAccessor>()))
        {
            var repository = new EfSoftDeleteRepository<SoftDeleteTestEntity>(context);
            
            // Act
            var result = await repository.GetByIdOrThrowAsync(entity.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(entity.Id);
            result.Name.Should().Be("Test Entity");
        }
    }
    
    [Fact]
    public async Task EfSoftDeleteRepository_GetByIdOrThrowAsync_SoftDeletedEntity_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        
        var entity = new SoftDeleteTestEntity { Id = Guid.NewGuid(), Name = "Test Entity", IsDeleted = true };
        
        using (var context = new ApplicationDbContext(options, Mock.Of<ICurrentUserAccessor>()))
        {
            await context.Set<SoftDeleteTestEntity>().AddAsync(entity);
            await context.SaveChangesAsync();
        }
        
        using (var context = new ApplicationDbContext(options, Mock.Of<ICurrentUserAccessor>()))
        {
            var repository = new EfSoftDeleteRepository<SoftDeleteTestEntity>(context);
            
            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => 
                repository.GetByIdOrThrowAsync(entity.Id));
        }
    }
    
    [Fact]
    public async Task EfSoftDeleteRepository_GetByIdOrThrowAsync_WithIncludeSoftDeleted_ShouldReturnSoftDeletedEntity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        
        var entity = new SoftDeleteTestEntity { Id = Guid.NewGuid(), Name = "Test Entity", IsDeleted = true };
        
        using (var context = new ApplicationDbContext(options, Mock.Of<ICurrentUserAccessor>()))
        {
            await context.Set<SoftDeleteTestEntity>().AddAsync(entity);
            await context.SaveChangesAsync();
        }
        
        using (var context = new ApplicationDbContext(options, Mock.Of<ICurrentUserAccessor>()))
        {
            var repository = new EfSoftDeleteRepository<SoftDeleteTestEntity>(context);
            
            // Act
            var result = await repository.GetByIdOrThrowAsync(entity.Id, true);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(entity.Id);
            result.IsDeleted.Should().BeTrue();
        }
    }
    
    [Fact]
    public async Task EfSoftDeleteRepository_GetByIdOrThrowAsync_NonExistingEntity_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
            
        var nonExistingId = Guid.NewGuid();
        
        using (var context = new ApplicationDbContext(options, Mock.Of<ICurrentUserAccessor>()))
        {
            var repository = new EfSoftDeleteRepository<SoftDeleteTestEntity>(context);
            
            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => 
                repository.GetByIdOrThrowAsync(nonExistingId, true));
        }
    }
}
