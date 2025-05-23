using Microsoft.EntityFrameworkCore;
using SoftwareEngineerSkills.Domain.Entities.Skills;
using SoftwareEngineerSkills.Domain.Enums;
using SoftwareEngineerSkills.Infrastructure.Persistence;
using SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;

namespace SoftwareEngineerSkills.Infrastructure.UnitTests.Persistence.Repositories;

public class SkillRepositoryTests
{
    private readonly Mock<ApplicationDbContext> _dbContextMock;
    private readonly Mock<DbSet<Skill>> _dbSetMock;
    private readonly SkillRepository _repository;
    private readonly List<Skill> _skills;

    public SkillRepositoryTests()
    {
        _skills = new List<Skill>
        {
            CreateSkill("C#", SkillCategory.ProgrammingLanguage, "C# description", SkillLevel.Advanced, true),
            CreateSkill("JavaScript", SkillCategory.ProgrammingLanguage, "JS description", SkillLevel.Intermediate, false),
            CreateSkill("React", SkillCategory.Framework, "React description", SkillLevel.Intermediate, true),
            CreateSkill("Entity Framework", SkillCategory.Framework, "EF description", SkillLevel.Beginner, false),
            CreateSkill("Azure", SkillCategory.Cloud, "Azure description", SkillLevel.Advanced, true)
        };

        _dbSetMock = MockDbSet(_skills);
        
        _dbContextMock = new Mock<ApplicationDbContext>();
        _dbContextMock.Setup(c => c.Set<Skill>()).Returns(_dbSetMock.Object);
        
        _repository = new SkillRepository(_dbContextMock.Object);
    }
    
    // Helper method to create skills using EF Core private constructor workaround
    private Skill CreateSkill(string name, SkillCategory category, string description, SkillLevel level, bool isInDemand)
    {
        // Create a skill instance using reflection or other workaround
        // In this test, we'll use mock data and assume the properties are properly set
        var skill = new Mock<Skill>().Object;
        
        // Use reflection to set the private fields
        typeof(Skill).GetProperty(nameof(Skill.Id))?.SetValue(skill, Guid.NewGuid());
        typeof(Skill).GetProperty(nameof(Skill.Name))?.SetValue(skill, name);
        typeof(Skill).GetProperty(nameof(Skill.Category))?.SetValue(skill, category);
        typeof(Skill).GetProperty(nameof(Skill.Description))?.SetValue(skill, description);
        typeof(Skill).GetProperty(nameof(Skill.DifficultyLevel))?.SetValue(skill, level);
        typeof(Skill).GetProperty(nameof(Skill.IsInDemand))?.SetValue(skill, isInDemand);
        
        return skill;
    }

    [Fact]
    public async Task GetByNameAsync_ExistingSkill_ShouldReturnSkill()
    {
        // Arrange
        var skillName = "C#";
        var expected = _skills.First();
        
        _dbSetMock.Setup(m => m.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Skill, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _repository.GetByNameAsync(skillName);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByNameAsync_NonExistingSkill_ShouldReturnNull()
    {
        // Arrange
        var skillName = "Non-Existent Skill";
        
        _dbSetMock.Setup(m => m.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Skill, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Skill?)null);

        // Act
        var result = await _repository.GetByNameAsync(skillName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCategoryAsync_ExistingCategory_ShouldReturnSkills()
    {
        // Arrange
        var category = SkillCategory.ProgrammingLanguage.ToString();
        var expected = _skills.Where(s => s.Category == SkillCategory.ProgrammingLanguage).ToList();
        
        var queryable = expected.AsQueryable();
        _dbSetMock.Setup(m => m.Where(It.IsAny<System.Linq.Expressions.Expression<Func<Skill, bool>>>()))
            .Returns(queryable);

        // Act
        var result = await _repository.GetByCategoryAsync(category);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByCategoryAsync_NonExistingCategory_ShouldReturnEmptyCollection()
    {
        // Arrange
        var category = "InvalidCategory";

        // Act
        var result = await _repository.GetByCategoryAsync(category);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByDifficultyLevelAsync_ExistingLevel_ShouldReturnSkills()
    {
        // Arrange
        var level = (int)SkillLevel.Advanced;
        var expected = _skills.Where(s => s.DifficultyLevel == SkillLevel.Advanced).ToList();
        
        var queryable = expected.AsQueryable();
        _dbSetMock.Setup(m => m.Where(It.IsAny<System.Linq.Expressions.Expression<Func<Skill, bool>>>()))
            .Returns(queryable);

        // Act
        var result = await _repository.GetByDifficultyLevelAsync(level);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByDifficultyLevelAsync_InvalidLevel_ShouldReturnEmptyCollection()
    {
        // Arrange
        var level = 99; // Invalid level

        // Act
        var result = await _repository.GetByDifficultyLevelAsync(level);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetInDemandSkillsAsync_ShouldReturnInDemandSkills()
    {
        // Arrange
        var expected = _skills.Where(s => s.IsInDemand).ToList();
        
        var queryable = expected.AsQueryable();
        _dbSetMock.Setup(m => m.Where(It.IsAny<System.Linq.Expressions.Expression<Func<Skill, bool>>>()))
            .Returns(queryable);

        // Act
        var result = await _repository.GetInDemandSkillsAsync();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(expected);
        result.Should().AllSatisfy(s => s.IsInDemand.Should().BeTrue());
    }

    [Fact]
    public async Task ExistsByNameAsync_ExistingSkill_ShouldReturnTrue()
    {
        // Arrange
        var skillName = "C#";
        
        _dbSetMock.Setup(m => m.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Skill, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _repository.ExistsByNameAsync(skillName);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByNameAsync_NonExistingSkill_ShouldReturnFalse()
    {
        // Arrange
        var skillName = "Non-Existent Skill";
        
        _dbSetMock.Setup(m => m.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Skill, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _repository.ExistsByNameAsync(skillName);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetPagedSkillsAsync_ValidParameters_ShouldReturnPagedResult()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 2;
        var totalCount = _skills.Count;
        var expected = _skills.OrderBy(s => s.Name).Take(pageSize).ToList();
        
        _dbSetMock.Setup(m => m.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);
            
        var orderedQueryable = expected.AsQueryable();
        _dbSetMock.As<IQueryable<Skill>>().Setup(m => m.Provider).Returns(orderedQueryable.Provider);
        _dbSetMock.As<IQueryable<Skill>>().Setup(m => m.Expression).Returns(orderedQueryable.Expression);
        _dbSetMock.As<IQueryable<Skill>>().Setup(m => m.ElementType).Returns(orderedQueryable.ElementType);
        _dbSetMock.As<IQueryable<Skill>>().Setup(m => m.GetEnumerator()).Returns(() => orderedQueryable.GetEnumerator());

        // Act
        var result = await _repository.GetPagedSkillsAsync(pageNumber, pageSize);

        // Assert
        result.Skills.Should().NotBeEmpty();
        result.Skills.Should().BeEquivalentTo(expected);
        result.TotalCount.Should().Be(totalCount);
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
