using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Common.Interfaces;

namespace SoftwareEngineerSkills.Infrastructure.UnitTests.TestHelpers;

/// <summary>
/// Simple entity class for testing repository operations
/// </summary>
public class TestEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public TestEntity() : base() { }
    
    public TestEntity(Guid id)
    {
        Id = id;
    }
}

/// <summary>
/// Test entity with soft delete functionality for testing soft delete repositories
/// </summary>
public class SoftDeleteTestEntity : BaseEntity, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    
    public SoftDeleteTestEntity() : base() { }
    
    public SoftDeleteTestEntity(Guid id)
    {
        Id = id;
    }
}
