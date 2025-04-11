using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using SoftwareEngineerSkills.Domain.Entities;
using SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;
using Xunit;

namespace SoftwareEngineerSkills.Tests.Infrastructure.Repositories;

public class DummyRepositoryTests
{
    private readonly DummyRepository _repository;

    public DummyRepositoryTests()
    {
        _repository = new DummyRepository();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveDummies_WhenIncludeInactiveIsFalse()
    {
        // Arrange
        var activeDummy = new Dummy { Id = Guid.NewGuid(), Name = "Active", IsActive = true };
        var inactiveDummy = new Dummy { Id = Guid.NewGuid(), Name = "Inactive", IsActive = false };
        await _repository.AddAsync(activeDummy);
        await _repository.AddAsync(inactiveDummy);

        // Act
        var result = await _repository.GetAllAsync(false);

        // Assert
        result.Should().ContainSingle(d => d.Id == activeDummy.Id);
        result.Should().NotContain(d => d.Id == inactiveDummy.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectDummy_WhenEntityExists()
    {
        // Arrange
        var dummy = new Dummy { Id = Guid.NewGuid(), Name = "Test", IsActive = true };
        await _repository.AddAsync(dummy);

        // Act
        var result = await _repository.GetByIdAsync(dummy.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(dummy.Id);
    }

    [Fact]
    public async Task AddAsync_AddsDummyToRepository()
    {
        // Arrange
        var dummy = new Dummy { Id = Guid.NewGuid(), Name = "Test", IsActive = true };

        // Act
        await _repository.AddAsync(dummy);
        var result = await _repository.GetByIdAsync(dummy.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(dummy.Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingDummy()
    {
        // Arrange
        var dummy = new Dummy { Id = Guid.NewGuid(), Name = "Test", IsActive = true };
        await _repository.AddAsync(dummy);
        dummy.Name = "Updated";

        // Act
        await _repository.UpdateAsync(dummy);
        var result = await _repository.GetByIdAsync(dummy.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteAsync_RemovesDummyFromRepository()
    {
        // Arrange
        var dummy = new Dummy { Id = Guid.NewGuid(), Name = "Test", IsActive = true };
        await _repository.AddAsync(dummy);

        // Act
        await _repository.DeleteAsync(dummy);
        var result = await _repository.GetByIdAsync(dummy.Id);

        // Assert
        result.Should().BeNull();
    }
}