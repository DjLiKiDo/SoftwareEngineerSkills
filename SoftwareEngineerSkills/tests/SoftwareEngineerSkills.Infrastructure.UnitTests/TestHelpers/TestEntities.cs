using SoftwareEngineerSkills.Domain.Common.Interfaces;
using System;

namespace SoftwareEngineerSkills.Infrastructure.UnitTests.TestHelpers;

/// <summary>
/// Simple entity class for testing repository operations
/// </summary>
public class TestEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Test entity with soft delete functionality for testing soft delete repositories
/// </summary>
public class SoftDeleteTestEntity : TestEntity, ISoftDelete
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
