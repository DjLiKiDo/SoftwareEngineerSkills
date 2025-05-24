using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Enums;

namespace SoftwareEngineerSkills.Domain.Aggregates.Developer;

/// <summary>
/// Event raised when a new developer is created
/// </summary>
public class DeveloperCreatedEvent : DomainEvent
{
    /// <summary>
    /// The ID of the newly created developer
    /// </summary>
    public Guid DeveloperId { get; }
    
    /// <summary>
    /// The name of the developer
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// The email address of the developer
    /// </summary>
    public string Email { get; }
    
    /// <summary>
    /// The overall experience level of the developer
    /// </summary>
    public SkillLevel ExperienceLevel { get; }
    
    /// <summary>
    /// Creates a new instance of the DeveloperCreatedEvent class
    /// </summary>
    /// <param name="developerId">The ID of the developer</param>
    /// <param name="name">The name of the developer</param>
    /// <param name="email">The email address of the developer</param>
    /// <param name="experienceLevel">The experience level of the developer</param>
    public DeveloperCreatedEvent(Guid developerId, string name, string email, SkillLevel experienceLevel)
    {
        DeveloperId = developerId;
        Name = name;
        Email = email;
        ExperienceLevel = experienceLevel;
    }
}

/// <summary>
/// Event raised when a developer's name is updated
/// </summary>
public class DeveloperNameUpdatedEvent : DomainEvent
{
    /// <summary>
    /// The ID of the developer whose name was updated
    /// </summary>
    public Guid DeveloperId { get; }
    
    /// <summary>
    /// The original name before the update
    /// </summary>
    public string OldName { get; }
    
    /// <summary>
    /// The new name after the update
    /// </summary>
    public string NewName { get; }
    
    /// <summary>
    /// Creates a new instance of the DeveloperNameUpdatedEvent class
    /// </summary>
    /// <param name="developerId">The ID of the developer</param>
    /// <param name="oldName">The original name</param>
    /// <param name="newName">The new name</param>
    public DeveloperNameUpdatedEvent(Guid developerId, string oldName, string newName)
    {
        DeveloperId = developerId;
        OldName = oldName;
        NewName = newName;
    }
}

/// <summary>
/// Event raised when a developer's email is updated
/// </summary>
public class DeveloperEmailUpdatedEvent : DomainEvent
{
    /// <summary>
    /// The ID of the developer whose email was updated
    /// </summary>
    public Guid DeveloperId { get; }
    
    /// <summary>
    /// The original email before the update
    /// </summary>
    public string OldEmail { get; }
    
    /// <summary>
    /// The new email after the update
    /// </summary>
    public string NewEmail { get; }
    
    /// <summary>
    /// Creates a new instance of the DeveloperEmailUpdatedEvent class
    /// </summary>
    /// <param name="developerId">The ID of the developer</param>
    /// <param name="oldEmail">The original email</param>
    /// <param name="newEmail">The new email</param>
    public DeveloperEmailUpdatedEvent(Guid developerId, string oldEmail, string newEmail)
    {
        DeveloperId = developerId;
        OldEmail = oldEmail;
        NewEmail = newEmail;
    }
}

/// <summary>
/// Event raised when a developer's experience level is updated
/// </summary>
public class DeveloperExperienceLevelUpdatedEvent : DomainEvent
{
    /// <summary>
    /// The ID of the developer whose experience level was updated
    /// </summary>
    public Guid DeveloperId { get; }
    
    /// <summary>
    /// The original experience level before the update
    /// </summary>
    public SkillLevel OldLevel { get; }
    
    /// <summary>
    /// The new experience level after the update
    /// </summary>
    public SkillLevel NewLevel { get; }
    
    /// <summary>
    /// Creates a new instance of the DeveloperExperienceLevelUpdatedEvent class
    /// </summary>
    /// <param name="developerId">The ID of the developer</param>
    /// <param name="oldLevel">The original experience level</param>
    /// <param name="newLevel">The new experience level</param>
    public DeveloperExperienceLevelUpdatedEvent(Guid developerId, SkillLevel oldLevel, SkillLevel newLevel)
    {
        DeveloperId = developerId;
        OldLevel = oldLevel;
        NewLevel = newLevel;
    }
}

/// <summary>
/// Event raised when a developer's bio is updated
/// </summary>
public class DeveloperBioUpdatedEvent : DomainEvent
{
    /// <summary>
    /// The ID of the developer whose bio was updated
    /// </summary>
    public Guid DeveloperId { get; }
    
    /// <summary>
    /// The original bio before the update
    /// </summary>
    public string? OldBio { get; }
    
    /// <summary>
    /// The new bio after the update
    /// </summary>
    public string? NewBio { get; }
    
    /// <summary>
    /// Creates a new instance of the DeveloperBioUpdatedEvent class
    /// </summary>
    /// <param name="developerId">The ID of the developer</param>
    /// <param name="oldBio">The original bio</param>
    /// <param name="newBio">The new bio</param>
    public DeveloperBioUpdatedEvent(Guid developerId, string? oldBio, string? newBio)
    {
        DeveloperId = developerId;
        OldBio = oldBio;
        NewBio = newBio;
    }
}

/// <summary>
/// Event raised when a skill is added to a developer
/// </summary>
public class SkillAddedToDeveloperEvent : DomainEvent
{
    /// <summary>
    /// The ID of the developer to whom the skill was added
    /// </summary>
    public Guid DeveloperId { get; }
    
    /// <summary>
    /// The name of the developer
    /// </summary>
    public string DeveloperName { get; }
    
    /// <summary>
    /// The ID of the added skill
    /// </summary>
    public Guid SkillId { get; }
    
    /// <summary>
    /// The name of the added skill
    /// </summary>
    public string SkillName { get; }
    
    /// <summary>
    /// The category of the added skill
    /// </summary>
    public SkillCategory SkillCategory { get; }
    
    /// <summary>
    /// The proficiency level of the developer in this skill
    /// </summary>
    public SkillLevel ProficiencyLevel { get; }
    
    /// <summary>
    /// Creates a new instance of the SkillAddedToDeveloperEvent class
    /// </summary>
    /// <param name="developerId">The ID of the developer</param>
    /// <param name="developerName">The name of the developer</param>
    /// <param name="skillId">The ID of the skill</param>
    /// <param name="skillName">The name of the skill</param>
    /// <param name="skillCategory">The category of the skill</param>
    /// <param name="proficiencyLevel">The proficiency level</param>
    public SkillAddedToDeveloperEvent(
        Guid developerId,
        string developerName,
        Guid skillId,
        string skillName,
        SkillCategory skillCategory,
        SkillLevel proficiencyLevel)
    {
        DeveloperId = developerId;
        DeveloperName = developerName;
        SkillId = skillId;
        SkillName = skillName;
        SkillCategory = skillCategory;
        ProficiencyLevel = proficiencyLevel;
    }
}

/// <summary>
/// Event raised when a skill is removed from a developer
/// </summary>
public class SkillRemovedFromDeveloperEvent : DomainEvent
{
    /// <summary>
    /// The ID of the developer from whom the skill was removed
    /// </summary>
    public Guid DeveloperId { get; }
    
    /// <summary>
    /// The name of the developer
    /// </summary>
    public string DeveloperName { get; }
    
    /// <summary>
    /// The ID of the removed skill
    /// </summary>
    public Guid SkillId { get; }
    
    /// <summary>
    /// The name of the removed skill
    /// </summary>
    public string SkillName { get; }
    
    /// <summary>
    /// Creates a new instance of the SkillRemovedFromDeveloperEvent class
    /// </summary>
    /// <param name="developerId">The ID of the developer</param>
    /// <param name="developerName">The name of the developer</param>
    /// <param name="skillId">The ID of the skill</param>
    /// <param name="skillName">The name of the skill</param>
    public SkillRemovedFromDeveloperEvent(
        Guid developerId,
        string developerName,
        Guid skillId,
        string skillName)
    {
        DeveloperId = developerId;
        DeveloperName = developerName;
        SkillId = skillId;
        SkillName = skillName;
    }
}

/// <summary>
/// Event raised when a developer's skill is updated
/// </summary>
public class DeveloperSkillUpdatedEvent : DomainEvent
{
    /// <summary>
    /// The ID of the developer whose skill was updated
    /// </summary>
    public Guid DeveloperId { get; }
    
    /// <summary>
    /// The name of the developer
    /// </summary>
    public string DeveloperName { get; }
    
    /// <summary>
    /// The ID of the updated skill
    /// </summary>
    public Guid SkillId { get; }
    
    /// <summary>
    /// The name of the updated skill
    /// </summary>
    public string SkillName { get; }
    
    /// <summary>
    /// The original proficiency level before the update
    /// </summary>
    public SkillLevel OldProficiencyLevel { get; }
    
    /// <summary>
    /// The new proficiency level after the update
    /// </summary>
    public SkillLevel NewProficiencyLevel { get; }
    
    /// <summary>
    /// The original years of experience before the update
    /// </summary>
    public int OldYearsOfExperience { get; }
    
    /// <summary>
    /// The new years of experience after the update
    /// </summary>
    public int NewYearsOfExperience { get; }
    
    /// <summary>
    /// The original endorsement status before the update
    /// </summary>
    public bool OldIsEndorsed { get; }
    
    /// <summary>
    /// The new endorsement status after the update
    /// </summary>
    public bool NewIsEndorsed { get; }
    
    /// <summary>
    /// Creates a new instance of the DeveloperSkillUpdatedEvent class
    /// </summary>
    /// <param name="developerId">The ID of the developer</param>
    /// <param name="developerName">The name of the developer</param>
    /// <param name="skillId">The ID of the skill</param>
    /// <param name="skillName">The name of the skill</param>
    /// <param name="oldProficiencyLevel">The original proficiency level</param>
    /// <param name="newProficiencyLevel">The new proficiency level</param>
    /// <param name="oldYearsOfExperience">The original years of experience</param>
    /// <param name="newYearsOfExperience">The new years of experience</param>
    /// <param name="oldIsEndorsed">The original endorsement status</param>
    /// <param name="newIsEndorsed">The new endorsement status</param>
    public DeveloperSkillUpdatedEvent(
        Guid developerId,
        string developerName,
        Guid skillId,
        string skillName,
        SkillLevel oldProficiencyLevel,
        SkillLevel newProficiencyLevel,
        int oldYearsOfExperience,
        int newYearsOfExperience,
        bool oldIsEndorsed,
        bool newIsEndorsed)
    {
        DeveloperId = developerId;
        DeveloperName = developerName;
        SkillId = skillId;
        SkillName = skillName;
        OldProficiencyLevel = oldProficiencyLevel;
        NewProficiencyLevel = newProficiencyLevel;
        OldYearsOfExperience = oldYearsOfExperience;
        NewYearsOfExperience = newYearsOfExperience;
        OldIsEndorsed = oldIsEndorsed;
        NewIsEndorsed = newIsEndorsed;
    }
}

/// <summary>
/// Event raised when a developer's skill is endorsed
/// </summary>
public class DeveloperSkillEndorsedEvent : DomainEvent
{
    /// <summary>
    /// The ID of the developer whose skill was endorsed
    /// </summary>
    public Guid DeveloperId { get; }
    
    /// <summary>
    /// The name of the developer
    /// </summary>
    public string DeveloperName { get; }
    
    /// <summary>
    /// The ID of the endorsed skill
    /// </summary>
    public Guid SkillId { get; }
    
    /// <summary>
    /// The name of the endorsed skill
    /// </summary>
    public string SkillName { get; }
    
    /// <summary>
    /// Creates a new instance of the DeveloperSkillEndorsedEvent class
    /// </summary>
    /// <param name="developerId">The ID of the developer</param>
    /// <param name="developerName">The name of the developer</param>
    /// <param name="skillId">The ID of the skill</param>
    /// <param name="skillName">The name of the skill</param>
    public DeveloperSkillEndorsedEvent(
        Guid developerId,
        string developerName,
        Guid skillId,
        string skillName)
    {
        DeveloperId = developerId;
        DeveloperName = developerName;
        SkillId = skillId;
        SkillName = skillName;
    }
}
