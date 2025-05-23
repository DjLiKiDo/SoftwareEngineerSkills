using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Enums;
using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Domain.Entities.Skills;

/// <summary>
/// Represents a software engineering skill or technology.
/// Implemented as a base entity rather than an aggregate root for simpler domain modeling.
/// </summary>
public class Skill : BaseEntity
{
    /// <summary>
    /// The name of the skill
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// The category of the skill
    /// </summary>
    public SkillCategory Category { get; private set; }
    
    /// <summary>
    /// A description of the skill
    /// </summary>
    public string Description { get; private set; } = string.Empty;
    
    /// <summary>
    /// The difficulty level of the skill
    /// </summary>
    public SkillLevel DifficultyLevel { get; private set; }
    
    /// <summary>
    /// Indicates whether the skill is currently in high demand in the job market
    /// </summary>
    public bool IsInDemand { get; private set; }
    
    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private Skill() { }
    
    /// <summary>
    /// Creates a new instance of the Skill class
    /// </summary>
    /// <param name="name">The name of the skill</param>
    /// <param name="category">The category of the skill</param>
    /// <param name="description">A description of the skill</param>
    /// <param name="difficultyLevel">The difficulty level of the skill</param>
    /// <param name="isInDemand">Whether the skill is in demand</param>
    public Skill(string name, SkillCategory category, string description, SkillLevel difficultyLevel, bool isInDemand = false)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        if (description == null)
            throw new ArgumentNullException(nameof(description));
            
        Name = name;
        Category = category;
        Description = description;
        DifficultyLevel = difficultyLevel;
        IsInDemand = isInDemand;
        
        AddDomainEvent(new SkillCreatedEvent(Id, name, category, category.ToString()));
        EnforceInvariants();
    }
    
    /// <summary>
    /// Updates the skill information
    /// </summary>
    /// <param name="name">The new name</param>
    /// <param name="category">The new category</param>
    /// <param name="description">The new description</param>
    /// <param name="difficultyLevel">The new difficulty level</param>
    /// <param name="isInDemand">The new in-demand status</param>
    public void Update(
        string name, 
        SkillCategory category, 
        string description, 
        SkillLevel difficultyLevel, 
        bool isInDemand)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        if (description == null)
            throw new ArgumentNullException(nameof(description));
            
        var originalName = Name; // Store original name before updating
        var originalCategory = Category;
        var wasInDemand = IsInDemand;
        
        Name = name;
        Category = category;
        Description = description;
        DifficultyLevel = difficultyLevel;
        IsInDemand = isInDemand;
        
        AddDomainEvent(new SkillUpdatedEvent(Id, originalName, name));
        EnforceInvariants();
        
        // Add additional domain events for significant changes
        if (originalCategory != category)
        {
            AddDomainEvent(new SkillCategoryChangedEvent(Id, Name, originalCategory, category));
        }
        
        if (wasInDemand != isInDemand)
        {
            AddDomainEvent(new SkillDemandChangedEvent(Id, Name, isInDemand));
        }
    }
    
    /// <summary>
    /// Sets the demand status of the skill
    /// </summary>
    /// <param name="isInDemand">The new demand status</param>
    public void SetDemandStatus(bool isInDemand)
    {
        if (IsInDemand != isInDemand)
        {
            IsInDemand = isInDemand;
            AddDomainEvent(new SkillDemandChangedEvent(Id, Name, isInDemand));
            EnforceInvariants();
        }
    }
    
    /// <summary>
    /// Updates the difficulty level of the skill
    /// </summary>
    /// <param name="level">The new difficulty level</param>
    public void UpdateDifficultyLevel(SkillLevel level)
    {
        if (DifficultyLevel != level)
        {
            var oldLevel = DifficultyLevel;
            DifficultyLevel = level;
            AddDomainEvent(new SkillDifficultyChangedEvent(Id, Name, oldLevel, level));
            EnforceInvariants();
        }
    }
    
    /// <summary>
    /// Validates the skill name
    /// </summary>
    /// <param name="name">The name to validate</param>
    /// <exception cref="BusinessRuleException">Thrown when the name is invalid</exception>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleException("Skill name cannot be empty");
        }
        
        if (name.Length > 100)
        {
            throw new BusinessRuleException("Skill name cannot exceed 100 characters");
        }
    }
    
    /// <summary>
    /// Validates the skill description
    /// </summary>
    /// <param name="description">The description to validate</param>
    /// <exception cref="BusinessRuleException">Thrown when the description is invalid</exception>
    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new BusinessRuleException("Skill description cannot be empty");
        }
        
        if (description.Length > 1000)
        {
            throw new BusinessRuleException("Skill description cannot exceed 1000 characters");
        }
    }
    
    /// <summary>
    /// Validates that the Skill entity satisfies all invariants
    /// </summary>
    /// <returns>A collection of error messages if any invariants are violated</returns>
    protected override IEnumerable<string> CheckInvariants()
    {
        // Name validation
        if (Name == null)
        {
            yield return "Skill name cannot be null";
        }
        else if (string.IsNullOrWhiteSpace(Name))
        {
            yield return "Skill name cannot be empty";
        }
        else if (Name.Length > 100)
        {
            yield return "Skill name cannot exceed 100 characters";
        }
        
        // Description validation
        if (Description == null)
        {
            yield return "Skill description cannot be null";
        }
        else if (string.IsNullOrWhiteSpace(Description))
        {
            yield return "Skill description cannot be empty";
        }
        else if (Description.Length > 1000)
        {
            yield return "Skill description cannot exceed 1000 characters";
        }
    }

    // Apply method removed as BaseEntity doesn't implement this pattern
    // Domain state changes are now managed directly in each method
}
