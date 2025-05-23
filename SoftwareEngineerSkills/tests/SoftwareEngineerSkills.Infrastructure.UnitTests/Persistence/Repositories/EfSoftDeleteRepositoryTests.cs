using Microsoft.EntityFrameworkCore;
using SoftwareEngineerSkills.Domain.DomainServices.Interfaces;
using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Common.Interfaces;
using SoftwareEngineerSkills.Infrastructure.Persistence;
using SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;

namespace SoftwareEngineerSkills.Infrastructure.UnitTests.Persistence.Repositories;

public class EfSoftDeleteRepositoryTests
{
    private readonly Mock<ApplicationDbContext> _dbContextMock;
    private readonly Mock<DbSet<TestSoftDeleteEntity>> _dbSetMock;
    private readonly EfSoftDeleteRepository<TestSoftDeleteEntity> _repository;
    private readonly List<TestSoftDeleteEntity> _entities;

    public EfSoftDeleteRepositoryTests()
    {
        _entities = new List<TestSoftDeleteEntity>
        {
            new TestSoftDeleteEntity { Name = "Entity 1", IsDeleted = false },
            new TestSoftDeleteEntity { Name = "Entity 2", IsDeleted = false },
            new TestSoftDeleteEntity { Name = "Entity 3 (Deleted)", IsDeleted = true, DeletedAt = DateTime.UtcNow.AddDays(-1) }
        };

        _dbSetMock = MockDbSet(_entities);
        
        _dbContextMock = new Mock<ApplicationDbContext>(
            new DbContextOptions<ApplicationDbContext>(), 
            Mock.Of<ICurrentUserService>()
        );
        
        _dbContextMock.Setup(c => c.Set<TestSoftDeleteEntity>()).Returns(_dbSetMock.Object);
        
        _repository = new EfSoftDeleteRepository<TestSoftDeleteEntity>(_dbContextMock.Object);
    }

    [Fact]
    public void SoftDelete_ValidEntity_ShouldUpdateEntityAndMarkAsDeleted()
    {
        // Arrange
        var entity = _entities[0];
        var deletedBy = "test-user";
        
        // Act
        _repository.SoftDelete(entity, deletedBy);
        
        // Assert
        entity.IsDeleted.Should().BeTrue();
        entity.DeletedBy.Should().Be(deletedBy);
        entity.DeletedAt.Should().NotBeNull();
        
        _dbContextMock.Verify(c => c.Update(entity), Times.Once);
    }

    [Fact]
    public void SoftDeleteRange_ValidEntities_ShouldUpdateAllEntitiesAndMarkAsDeleted()
    {
        // Arrange
        var entities = _entities.Take(2).ToList();
        var deletedBy = "test-user";
        
        // Act
        _repository.SoftDeleteRange(entities, deletedBy);
        
        // Assert
        foreach (var entity in entities)
        {
            entity.IsDeleted.Should().BeTrue();
            entity.DeletedBy.Should().Be(deletedBy);
            entity.DeletedAt.Should().NotBeNull();
        }
        
        _dbContextMock.Verify(c => c.Update(It.IsAny<TestSoftDeleteEntity>()), Times.Exactly(entities.Count));
    }

    [Fact]
    public void Restore_SoftDeletedEntity_ShouldUpdateAndClearDeletedFlags()
    {
        // Arrange
        var entity = _entities[2]; // Already soft deleted
        
        // Act
        _repository.Restore(entity);
        
        // Assert
        entity.IsDeleted.Should().BeFalse();
        entity.DeletedBy.Should().BeNull();
        entity.DeletedAt.Should().BeNull();
        
        _dbContextMock.Verify(c => c.Update(entity), Times.Once);
    }

    [Fact]
    public async Task GetSoftDeletedAsync_ShouldReturnOnlySoftDeletedEntities()
    {
        // Arrange
        var softDeletedEntities = _entities.Where(e => e.IsDeleted).ToList();
        var mockQueryable = softDeletedEntities.AsQueryable();
        
        _dbSetMock
            .Setup(m => m.IgnoreQueryFilters())
            .Returns(_dbSetMock.Object);
            
        _dbSetMock
            .Setup(m => m.Where(It.IsAny<System.Linq.Expressions.Expression<Func<TestSoftDeleteEntity, bool>>>()))
            .Returns(mockQueryable);

        // Act
        var result = await _repository.GetSoftDeletedAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(softDeletedEntities.Count);
        result.Should().AllSatisfy(e => e.IsDeleted.Should().BeTrue());
    }

    [Fact]
    public async Task GetByIdAsync_ExcludingSoftDeleted_ShouldFilterOutDeletedEntities()
    {
        // Arrange
        var entityId = _entities[0].Id;
        
        _dbSetMock
            .Setup(m => m.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<TestSoftDeleteEntity, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_entities[0]);

        // Act
        var result = await _repository.GetByIdAsync(entityId, false);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(_entities[0]);
    }

    [Fact]
    public async Task GetByIdAsync_IncludingSoftDeleted_ShouldReturnDeletedEntities()
    {
        // Arrange
        var entityId = _entities[2].Id; // Soft deleted entity
        
        _dbSetMock
            .Setup(m => m.IgnoreQueryFilters())
            .Returns(_dbSetMock.Object);
            
        _dbSetMock
            .Setup(m => m.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<TestSoftDeleteEntity, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_entities[2]);

        // Act
        var result = await _repository.GetByIdAsync(entityId, true);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(_entities[2]);
        result!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Remove_ShouldUseSoftDelete()
    {
        // Arrange
        var entity = _entities[0];
        
        // Act
        _repository.Remove(entity);
        
        // Assert
        entity.IsDeleted.Should().BeTrue();
        entity.DeletedAt.Should().NotBeNull();
        
        _dbContextMock.Verify(c => c.Update(entity), Times.Once);
    }

    [Fact]
    public void RemoveRange_ShouldUseSoftDelete()
    {
        // Arrange
        var entities = _entities.Take(2).ToList();
        
        // Act
        _repository.RemoveRange(entities);
        
        // Assert
        foreach (var entity in entities)
        {
            entity.IsDeleted.Should().BeTrue();
            entity.DeletedAt.Should().NotBeNull();
        }
        
        _dbContextMock.Verify(c => c.Update(It.IsAny<TestSoftDeleteEntity>()), Times.Exactly(entities.Count));
    }

    // Helper class for testing
    public class TestSoftDeleteEntity : BaseEntity, ISoftDelete
    {
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }

    // Helper methods to create DbSet mocks
    private static Mock<DbSet<T>> MockDbSet<T>(IEnumerable<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var dbSetMock = new Mock<DbSet<T>>();
        
        dbSetMock.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(queryable.Provider);
            
        dbSetMock.As<IQueryable<T>>()
            .Setup(m => m.Expression)
            .Returns(queryable.Expression);
            
        dbSetMock.As<IQueryable<T>>()
            .Setup(m => m.ElementType)
            .Returns(queryable.ElementType);
            
        dbSetMock.As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator())
            .Returns(() => queryable.GetEnumerator());
            
        return dbSetMock;
    }
}
