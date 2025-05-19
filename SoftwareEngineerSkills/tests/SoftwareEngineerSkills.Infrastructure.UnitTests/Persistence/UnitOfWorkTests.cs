using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Abstractions.Services;
using SoftwareEngineerSkills.Infrastructure.Persistence;
using SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoftwareEngineerSkills.Infrastructure.UnitTests.Persistence;

public class UnitOfWorkTests
{
    private readonly Mock<ApplicationDbContext> _dbContextMock;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        _dbContextMock = new Mock<ApplicationDbContext>(
            new DbContextOptions<ApplicationDbContext>(), 
            Mock.Of<ICurrentUserService>());
        
        _unitOfWork = new UnitOfWork(_dbContextMock.Object);
    }

    [Fact]
    public void Constructor_NullContext_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Action act = () => new UnitOfWork(null!);
        
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("context");
    }

    [Fact]
    public void Skills_ShouldReturnSkillRepository()
    {
        // Act
        var repository = _unitOfWork.Skills;
        
        // Assert
        repository.Should().NotBeNull();
        repository.Should().BeAssignableTo<ISkillRepository>();
    }

    [Fact]
    public void Skills_MultipleCalls_ShouldReturnSameInstance()
    {
        // Act
        var repository1 = _unitOfWork.Skills;
        var repository2 = _unitOfWork.Skills;
        
        // Assert
        repository1.Should().BeSameAs(repository2);
    }

    [Fact]
    public async Task CommitAsync_ShouldCallSaveChangesAsync()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _dbContextMock
            .Setup(c => c.SaveChangesAsync(cancellationToken))
            .ReturnsAsync(5); // 5 changes saved
        
        // Act
        var result = await _unitOfWork.CommitAsync(cancellationToken);
        
        // Assert
        result.Should().Be(5);
        _dbContextMock.Verify(c => c.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public void Rollback_ShouldClearChangeTracker()
    {
        // Arrange
        var changeTrackerMock = new Mock<Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker>();
        _dbContextMock.Setup(c => c.ChangeTracker).Returns(changeTrackerMock.Object);
        
        // Act
        _unitOfWork.Rollback();
        
        // Assert
        changeTrackerMock.Verify(c => c.Clear(), Times.Once);
    }

    [Fact]
    public async Task BeginTransactionAsync_ShouldCallContextBeginTransactionAsync()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        
        // Act
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        // Assert
        _dbContextMock.Verify(c => c.BeginTransactionAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CommitTransactionAsync_ShouldCallContextCommitTransactionAsync()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        
        // Act
        await _unitOfWork.CommitTransactionAsync(cancellationToken);
        
        // Assert
        _dbContextMock.Verify(c => c.CommitTransactionAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RollbackTransactionAsync_ShouldCallContextRollbackTransactionAsync()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        
        // Act
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
        
        // Assert
        _dbContextMock.Verify(c => c.RollbackTransactionAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public void Dispose_ShouldDisposeContext()
    {
        // Act
        _unitOfWork.Dispose();
        
        // Assert
        _dbContextMock.Verify(c => c.Dispose(), Times.Once);
    }
    
    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldOnlyDisposeContextOnce()
    {
        // Act
        _unitOfWork.Dispose();
        _unitOfWork.Dispose();
        
        // Assert
        _dbContextMock.Verify(c => c.Dispose(), Times.Once);
    }
}
