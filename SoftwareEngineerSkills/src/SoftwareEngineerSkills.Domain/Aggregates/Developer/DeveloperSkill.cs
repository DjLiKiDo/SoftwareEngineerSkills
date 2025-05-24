using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Enums;
using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Domain.Aggregates.Developer;

/// <summary>
/// Represents a skill within the Developer aggregate boundary.
/// This is a child entity that can only be managed through the Developer aggregate root.
/// </summary>
/// <remarks>
/// <para>
/// DeveloperSkill is designed as a child entity within the Developer aggregate.
/// It cannot exist independently and all operations must go through the Developer
/// aggregate root to maintain consistency and enforce business rules.
/// </para>
/// <para>
/// Key characteristics:
/// - Immutable after creation (except through aggregate root methods)
/// - Contains developer-specific skill information (proficiency, experience, endorsements)
/// - Validates its own consistency through invariant checking
/// - Does not generate domain events directly (handled by aggregate root)
/// </para>
/// </remarks>
public class DeveloperSkill : BaseEntity
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
    /// The developer's proficiency level in this skill
    /// </summary>
    public SkillLevel ProficiencyLevel { get; private set; }
    
    /// <summary>
    /// Optional description of the developer's expertise in this skill
    /// </summary>
    public string? Description { get; private set; }
    
    /// <summary>
    /// Number of years of experience with this skill
    /// </summary>
    public int YearsOfExperience { get; private set; }
    
    /// <summary>
    /// Indicates whether this skill has been endorsed by peers or employers
    /// </summary>
    public bool IsEndorsed { get; private set; }
    
    /// <summary>
    /// The date when the skill was last updated
    /// </summary>
    public DateTime LastUpdated { get; private set; }
    
    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private DeveloperSkill() { }
    
    /// <summary>
    /// Creates a new developer skill instance.
    /// This constructor is internal and should only be called by the Developer aggregate root.
    /// </summary>
    /// <param name="name">The name of the skill</param>
    /// <param name="category">The skill category</param>
    /// <param name="proficiencyLevel">The developer's proficiency level</param>
    /// <param name="description">Optional description of expertise</param>
    /// <param name="yearsOfExperience">Years of experience with this skill</param>
    /// <param name="isEndorsed">Whether the skill is endorsed</param>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    internal DeveloperSkill(
        string name,
        SkillCategory category,
        SkillLevel proficiencyLevel,
        string? description = null,
        int yearsOfExperience = 0,
        bool isEndorsed = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Skill name cannot be empty", nameof(name));
        
        if (yearsOfExperience < 0)
            throw new ArgumentException("Years of experience cannot be negative", nameof(yearsOfExperience));
        
        Name = name.Trim();
        Category = category;
        ProficiencyLevel = proficiencyLevel;
        Description = description?.Trim();
        YearsOfExperience = yearsOfExperience;
        IsEndorsed = isEndorsed;
        LastUpdated = DateTime.UtcNow;
        
        EnforceInvariants();
    }
    
    /// <summary>
    /// Updates the proficiency information for this skill.
    /// This method is internal and should only be called by the Developer aggregate root.
    /// </summary>
    /// <param name="proficiencyLevel">The new proficiency level</param>
    /// <param name="description">The new description</param>
    /// <param name="yearsOfExperience">The new years of experience</param>
    /// <param name="isEndorsed">The new endorsement status</param>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    internal void UpdateProficiency(
        SkillLevel proficiencyLevel,
        string? description = null,
        int yearsOfExperience = 0,
        bool isEndorsed = false)
    {
        if (yearsOfExperience < 0)
            throw new ArgumentException("Years of experience cannot be negative", nameof(yearsOfExperience));
        
        ProficiencyLevel = proficiencyLevel;
        Description = description?.Trim();
        YearsOfExperience = yearsOfExperience;
        IsEndorsed = isEndorsed;
        LastUpdated = DateTime.UtcNow;
        
        EnforceInvariants();
    }
    
    /// <summary>
    /// Endorses this skill.
    /// This method is internal and should only be called by the Developer aggregate root.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the skill is already endorsed</exception>
    internal void Endorse()
    {
        if (IsEndorsed)
            throw new InvalidOperationException("Skill is already endorsed");
        
        IsEndorsed = true;
        LastUpdated = DateTime.UtcNow;
        
        EnforceInvariants();
    }
    
    /// <summary>
    /// Removes endorsement from this skill.
    /// This method is internal and should only be called by the Developer aggregate root.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the skill is not endorsed</exception>
    internal void RemoveEndorsement()
    {
        if (!IsEndorsed)
            throw new InvalidOperationException("Skill is not endorsed");
        
        IsEndorsed = false;
        LastUpdated = DateTime.UtcNow;
        
        EnforceInvariants();
    }
    
    /// <summary>
    /// Determines if this skill is considered advanced based on proficiency and experience
    /// </summary>
    /// <returns>True if the skill is considered advanced, false otherwise</returns>
    public bool IsAdvanced()
    {
        return ProficiencyLevel >= SkillLevel.Advanced || YearsOfExperience >= 5;
    }
    
    /// <summary>
    /// Determines if this skill needs attention (low proficiency with significant experience)
    /// </summary>
    /// <returns>True if the skill needs attention, false otherwise</returns>
    public bool NeedsAttention()
    {
        return ProficiencyLevel == SkillLevel.Beginner && YearsOfExperience >= 2;
    }
    
    /// <summary>
    /// Gets a display-friendly summary of the skill
    /// </summary>
    /// <returns>A formatted string representing the skill summary</returns>
    public string GetSummary()
    {
        var endorsedText = IsEndorsed ? " (Endorsed)" : "";
        var experienceText = YearsOfExperience > 0 ? $", {YearsOfExperience} years" : "";
        
        return $"{Name} - {ProficiencyLevel}{experienceText}{endorsedText}";
    }
    
    /// <summary>
    /// Validates that the DeveloperSkill entity satisfies all invariants
    /// </summary>
    /// <returns>A collection of error messages if any invariants are violated</returns>
    protected override IEnumerable<string> CheckInvariants()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            yield return "Skill name cannot be empty";
        }
        else if (Name.Length > 100)
        {
            yield return "Skill name cannot exceed 100 characters";
        }
        
        if (Description?.Length > 1000)
        {
            yield return "Skill description cannot exceed 1000 characters";
        }
        
        if (YearsOfExperience < 0)
        {
            yield return "Years of experience cannot be negative";
        }
        
        if (YearsOfExperience > 50)
        {
            yield return "Years of experience seems unrealistic (maximum 50 years)";
        }
        
        // Business rule: Advanced skills should have some experience
        if (ProficiencyLevel == SkillLevel.Expert && YearsOfExperience == 0)
        {
            yield return "Expert level skills should have at least some experience";
        }
        
        // Business rule: Endorsed skills should have reasonable proficiency
        if (IsEndorsed && ProficiencyLevel == SkillLevel.Beginner && YearsOfExperience == 0)
        {
            yield return "Endorsed skills should demonstrate some proficiency or experience";
        }
    }
    
    /// <summary>
    /// Exposes the CheckInvariants method for aggregate root validation
    /// </summary>
    /// <returns>A collection of error messages if any invariants are violated</returns>
    internal IEnumerable<string> ValidateInvariants()
    {
        return CheckInvariants();
    }
}
