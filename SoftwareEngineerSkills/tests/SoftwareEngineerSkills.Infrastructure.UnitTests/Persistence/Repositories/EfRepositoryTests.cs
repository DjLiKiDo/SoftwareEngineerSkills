using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SoftwareEngineerSkills.Domain.Entities.Skills;
using SoftwareEngineerSkills.Domain.Enums;
using SoftwareEngineerSkills.Infrastructure.Persistence;
using SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;

namespace SoftwareEngineerSkills.Infrastructure.UnitTests.Persistence.Repositories;

public class EfRepositoryTests
{
    private readonly Mock<ApplicationDbContext> _dbContextMock;
    private readonly Mock<DbSet<TestEntity>> _dbSetMock;
    private readonly EfRepository<TestEntity> _repository;
    private readonly List<TestEntity> _entities;

    public EfRepositoryTests()
    {
        _entities = new List<TestEntity>
        {
            new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
            new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" },
            new TestEntity { Id = Guid.NewGuid(), Name = "Entity 3" }
        };

        _dbSetMock = MockDbSet(_entities);
        
        _dbContextMock = new Mock<ApplicationDbContext>();
        _dbContextMock.Setup(c => c.Set<TestEntity>()).Returns(_dbSetMock.Object);
        
        _repository = new EfRepository<TestEntity>(_dbContextMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingEntity_ShouldReturnEntity()
    {
        // Arrange
        var entityId = _entities[0].Id;
        
        _dbSetMock
            .Setup(m => m.FindAsync(new object[] { entityId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_entities[0]);

        // Act
        var result = await _repository.GetByIdAsync(entityId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(_entities[0]);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingEntity_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        
        _dbSetMock
            .Setup(m => m.FindAsync(new object[] { nonExistingId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestEntity?)null);

        // Act
        var result = await _repository.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(_entities);
    }

    [Fact]
    public async Task FindAsync_WithPredicate_ShouldReturnFilteredEntities()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> predicate = e => e.Name.Contains("2");

        // Act
        var result = await _repository.FindAsync(predicate);

        // Assert
        result.Should().ContainSingle();
        result.First().Name.Should().Be("Entity 2");
    }

    [Fact]
    public async Task AnyAsync_WithMatchingPredicate_ShouldReturnTrue()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> predicate = e => e.Name.Contains("Entity");
        
        _dbSetMock
            .Setup(m => m.AnyAsync(predicate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _repository.AnyAsync(predicate);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AnyAsync_WithNonMatchingPredicate_ShouldReturnFalse()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> predicate = e => e.Name.Contains("NonExistent");
        
        _dbSetMock
            .Setup(m => m.AnyAsync(predicate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _repository.AnyAsync(predicate);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CountAsync_WithoutPredicate_ShouldReturnTotalCount()
    {
        // Arrange
        _dbSetMock
            .Setup(m => m.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_entities.Count);

        // Act
        var result = await _repository.CountAsync();

        // Assert
        result.Should().Be(_entities.Count);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ShouldReturnFilteredCount()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> predicate = e => e.Name.Contains("1");
        
        _dbSetMock
            .Setup(m => m.CountAsync(predicate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _repository.CountAsync(predicate);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task AddAsync_ValidEntity_ShouldAddToDbSet()
    {
        // Arrange
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "New Entity" };

        // Act
        await _repository.AddAsync(entity);

        // Assert
        _dbSetMock.Verify(m => m.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddRangeAsync_ValidEntities_ShouldAddToDbSet()
    {
        // Arrange
        var entities = new List<TestEntity>
        {
            new TestEntity { Id = Guid.NewGuid(), Name = "New Entity 1" },
            new TestEntity { Id = Guid.NewGuid(), Name = "New Entity 2" }
        };

        // Act
        await _repository.AddRangeAsync(entities);

        // Assert
        _dbSetMock.Verify(m => m.AddRangeAsync(entities, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Update_ValidEntity_ShouldUpdateInDbSet()
    {
        // Arrange
        var entity = _entities[0];

        // Act
        _repository.Update(entity);

        // Assert
        _dbSetMock.Verify(m => m.Update(entity), Times.Once);
    }

    [Fact]
    public void Remove_ValidEntity_ShouldRemoveFromDbSet()
    {
        // Arrange
        var entity = _entities[0];

        // Act
        _repository.Remove(entity);

        // Assert
        _dbSetMock.Verify(m => m.Remove(entity), Times.Once);
    }

    [Fact]
    public void RemoveRange_ValidEntities_ShouldRemoveFromDbSet()
    {
        // Arrange
        var entities = _entities.Take(2).ToList();

        // Act
        _repository.RemoveRange(entities);

        // Assert
        _dbSetMock.Verify(m => m.RemoveRange(entities), Times.Once);
    }

    // Helper methods to create DbSet mocks
    private static Mock<DbSet<T>> MockDbSet<T>(IEnumerable<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var dbSetMock = new Mock<DbSet<T>>();
        
        dbSetMock.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
            
        dbSetMock.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
            
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

    // Test entity for repository
    public class TestEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    
    // Helper classes for async queries
    private class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object? Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new TestAsyncEnumerable<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var resultType = typeof(TResult).GetGenericArguments()[0];
            var executeMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethods()
                .First(m => m.Name == "ToListAsync" && m.GetParameters().Any(p => p.ParameterType == typeof(CancellationToken)))
                .MakeGenericMethod(resultType);

            return (TResult)executeMethod.Invoke(null, new object[] { _inner.CreateQuery(expression), cancellationToken })!;
        }
    }

    private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this.Provider);
    }

    private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current => _inner.Current;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
