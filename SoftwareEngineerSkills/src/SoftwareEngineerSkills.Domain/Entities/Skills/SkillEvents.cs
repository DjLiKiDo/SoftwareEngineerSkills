using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Enums;

namespace SoftwareEngineerSkills.Domain.Entities.Skills;

/// <summary>
/// Event raised when a new skill is created
/// </summary>
public class SkillCreatedEvent : DomainEvent
{
    /// <summary>
    /// The ID of the newly created skill
    /// </summary>
    public Guid SkillId { get; }
    
    /// <summary>
    /// The name of the newly created skill
    /// </summary>
    public string SkillName { get; }
    
    /// <summary>
    /// The string representation of the skill's category
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// The enumeration value of the skill's category
    /// </summary>
    public SkillCategory SkillCategory { get; }

    /// <summary>
    /// Creates a new instance of the SkillCreatedEvent class
    /// </summary>
    /// <param name="skillId">The ID of the skill</param>
    /// <param name="skillName">The name of the skill</param>
    /// <param name="skillCategory">The skill category of the skill</param>
    /// <param name="category">The category of the skill as a string</param>
    public SkillCreatedEvent(Guid skillId, string skillName, SkillCategory skillCategory, string category)
    {
        SkillId = skillId;
        SkillName = skillName;
        SkillCategory = skillCategory;
        Category = category;
    }
}

/// <summary>
/// Event raised when a skill is updated
/// </summary>
public class SkillUpdatedEvent : DomainEvent
{
    /// <summary>
    /// The ID of the skill that was updated
    /// </summary>
    public Guid SkillId { get; }
    
    /// <summary>
    /// The original name of the skill before the update
    /// </summary>
    public string OldName { get; }

    /// <summary>
    /// The new name of the skill after the update
    /// </summary>
    public string NewName { get; }

    /// <summary>
    /// Creates a new instance of the SkillUpdatedEvent class
    /// </summary>
    /// <param name="skillId">The ID of the skill</param>
    /// <param name="oldName">The original name of the skill</param>
    /// <param name="newName">The new name of the skill</param>
    public SkillUpdatedEvent(Guid skillId, string oldName, string newName)
    {
        SkillId = skillId;
        OldName = oldName;
        NewName = newName;
    }
}

/// <summary>
/// Event raised when a skill's demand status changes
/// </summary>
public class SkillDemandChangedEvent : DomainEvent
{
    /// <summary>
    /// The ID of the skill whose demand status changed
    /// </summary>
    public Guid SkillId { get; }
    
    /// <summary>
    /// The name of the skill whose demand status changed
    /// </summary>
    public string SkillName { get; }
    
    /// <summary>
    /// The new demand status value
    /// </summary>
    public bool IsInDemand { get; }
    
    /// <summary>
    /// Creates a new instance of the SkillDemandChangedEvent class
    /// </summary>
    /// <param name="skillId">The ID of the skill</param>
    /// <param name="skillName">The name of the skill</param>
    /// <param name="isInDemand">The new demand status</param>
    public SkillDemandChangedEvent(Guid skillId, string skillName, bool isInDemand)
    {
        SkillId = skillId;
        SkillName = skillName;
        IsInDemand = isInDemand;
    }
}

/// <summary>
/// Event raised when a skill's category changes
/// </summary>
public class SkillCategoryChangedEvent : DomainEvent
{
    /// <summary>
    /// The ID of the skill whose category changed
    /// </summary>
    public Guid SkillId { get; }
    
    /// <summary>
    /// The name of the skill whose category changed
    /// </summary>
    public string SkillName { get; }
    
    /// <summary>
    /// The original category before the change
    /// </summary>
    public SkillCategory OldCategory { get; }
    
    /// <summary>
    /// The new category after the change
    /// </summary>
    public SkillCategory NewCategory { get; }
    
    /// <summary>
    /// Creates a new instance of the SkillCategoryChangedEvent class
    /// </summary>
    /// <param name="skillId">The ID of the skill</param>
    /// <param name="skillName">The name of the skill</param>
    /// <param name="oldCategory">The original category</param>
    /// <param name="newCategory">The new category</param>
    public SkillCategoryChangedEvent(Guid skillId, string skillName, SkillCategory oldCategory, SkillCategory newCategory)
    {
        SkillId = skillId;
        SkillName = skillName;
        OldCategory = oldCategory;
        NewCategory = newCategory;
    }
}

/// <summary>
/// Event raised when a skill's difficulty level changes
/// </summary>
public class SkillDifficultyChangedEvent : DomainEvent
{
    /// <summary>
    /// The ID of the skill whose difficulty level changed
    /// </summary>
    public Guid SkillId { get; }
    
    /// <summary>
    /// The name of the skill whose difficulty level changed
    /// </summary>
    public string SkillName { get; }
    
    /// <summary>
    /// The original difficulty level before the change
    /// </summary>
    public SkillLevel OldLevel { get; }
    
    /// <summary>
    /// The new difficulty level after the change
    /// </summary>
    public SkillLevel NewLevel { get; }
    
    /// <summary>
    /// Creates a new instance of the SkillDifficultyChangedEvent class
    /// </summary>
    /// <param name="skillId">The ID of the skill</param>
    /// <param name="skillName">The name of the skill</param>
    /// <param name="oldLevel">The original difficulty level</param>
    /// <param name="newLevel">The new difficulty level</param>
    public SkillDifficultyChangedEvent(Guid skillId, string skillName, SkillLevel oldLevel, SkillLevel newLevel)
    {
        SkillId = skillId;
        SkillName = skillName;
        OldLevel = oldLevel;
        NewLevel = newLevel;
    }
}
