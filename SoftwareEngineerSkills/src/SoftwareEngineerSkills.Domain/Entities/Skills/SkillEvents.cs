using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Enums;

namespace SoftwareEngineerSkills.Domain.Entities.Skills;

/// <summary>
/// Event raised when a new skill is created
/// </summary>
public class SkillCreatedEvent : DomainEvent, IDomainEvent
{
    /// <summary>
    /// The ID of the skill that was created
    /// </summary>
    public Guid SkillId { get; }
    
    /// <summary>
    /// The name of the skill that was created
    /// </summary>
    public string SkillName { get; }
    
    /// <summary>
    /// The category of the skill that was created
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// The skill category of the skill that was created
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
public class SkillUpdatedEvent : DomainEvent, IDomainEvent
{
    /// <summary>
    /// The ID of the skill that was updated
    /// </summary>
    public Guid SkillId { get; }
    
    /// <summary>
    /// The name of the skill that was updated
    /// </summary>
    public string SkillName { get; }

    /// <summary>
    /// The new name of the skill
    /// </summary>
    public string NewName { get; } 

    /// <summary>
    /// Creates a new instance of the SkillUpdatedEvent class
    /// </summary>
    /// <param name="skillId">The ID of the skill</param>
    /// <param name="skillName">The original name of the skill</param>
    /// <param name="newName">The new name of the skill</param>
    public SkillUpdatedEvent(Guid skillId, string skillName, string newName)
    {
        SkillId = skillId;
        SkillName = skillName; // This is the original name before update for context
        NewName = newName;
    }
}

/// <summary>
/// Event raised when a skill's demand status changes
/// </summary>
public class SkillDemandChangedEvent : DomainEvent, IDomainEvent
{
    /// <summary>
    /// The ID of the skill
    /// </summary>
    public Guid SkillId { get; }
    
    /// <summary>
    /// The name of the skill
    /// </summary>
    public string SkillName { get; }
    
    /// <summary>
    /// The new demand status
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
