using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Enums;
using SoftwareEngineerSkills.Domain.Exceptions;
using SoftwareEngineerSkills.Domain.ValueObjects;

namespace SoftwareEngineerSkills.Domain.Aggregates.Developer;

/// <summary>
/// Developer aggregate root entity representing a software developer with their skills collection.
/// This aggregate encapsulates a developer's identity and their associated skills,
/// maintaining consistency within the aggregate boundary.
/// </summary>
/// <remarks>
/// <para>
/// The Developer aggregate follows Domain-Driven Design principles where the Developer
/// acts as the aggregate root and Skills are managed as child entities within the
/// aggregate boundary. This ensures that all skill operations maintain aggregate consistency.
/// </para>
/// <para>
/// Key responsibilities:
/// - Managing the developer's profile information (name, email, experience level)
/// - Maintaining the collection of skills with business rules
/// - Ensuring no duplicate skills exist for the same developer
/// - Coordinating skill-related domain events
/// - Enforcing aggregate invariants and consistency rules
/// </para>
/// </remarks>
public class Developer : AggregateRoot
{
    /// <summary>
    /// The developer's full name
    /// </summary>
    public string Name { get; private set; } = null!;
    
    /// <summary>
    /// The developer's email address
    /// </summary>
    public Email Email { get; private set; } = null!;
    
    /// <summary>
    /// The developer's overall experience level
    /// </summary>
    public SkillLevel ExperienceLevel { get; private set; }
    
    /// <summary>
    /// Optional bio or description of the developer
    /// </summary>
    public string? Bio { get; private set; }
    
    /// <summary>
    /// Backing field for the skills collection
    /// </summary>
    private readonly List<DeveloperSkill> _skills = new();
    
    /// <summary>
    /// Read-only collection of the developer's skills
    /// </summary>
    public IReadOnlyCollection<DeveloperSkill> Skills => _skills.AsReadOnly();
    
    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private Developer() { }
    
    /// <summary>
    /// Creates a new developer instance
    /// </summary>
    /// <param name="name">The developer's full name</param>
    /// <param name="email">The developer's email address</param>
    /// <param name="experienceLevel">The developer's overall experience level</param>
    /// <param name="bio">Optional bio or description</param>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
    /// <exception cref="DomainValidationException">Thrown when domain rules are violated</exception>
    public Developer(string name, Email email, SkillLevel experienceLevel, string? bio = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        ExperienceLevel = experienceLevel;
        Bio = bio;
        
        // Set audit properties for testing
        Created = DateTime.UtcNow;
        
        AddAndApplyEvent(new DeveloperCreatedEvent(Id, name, email.Value, experienceLevel));
    }
    
    /// <summary>
    /// Updates the developer's profile information
    /// </summary>
    /// <param name="name">The new name</param>
    /// <param name="email">The new email address</param>
    /// <param name="experienceLevel">The new experience level</param>
    /// <param name="bio">The new bio</param>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
    /// <exception cref="DomainValidationException">Thrown when domain rules are violated</exception>
    public void UpdateProfile(string name, Email email, SkillLevel experienceLevel, string? bio = null)
    {
        var newName = name ?? throw new ArgumentNullException(nameof(name));
        var newEmail = email ?? throw new ArgumentNullException(nameof(email));
        
        // Early validation for immediate feedback
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new DomainValidationException(["Developer name cannot be empty"]);
        }
        
        if (newName.Length > 200)
        {
            throw new DomainValidationException(["Developer name cannot exceed 200 characters"]);
        }
        
        // Track changes for events
        var nameChanged = Name != newName;
        var emailChanged = Email?.Value != newEmail.Value;
        var experienceLevelChanged = ExperienceLevel != experienceLevel;
        var bioChanged = Bio != bio;
        
        // Store old values for events
        var oldName = Name;
        var oldEmail = Email;
        var oldExperienceLevel = ExperienceLevel;
        var oldBio = Bio;
        
        // Update properties
        Name = newName;
        Email = newEmail;
        ExperienceLevel = experienceLevel;
        Bio = bio;
        
        // Generate appropriate events
        if (nameChanged)
        {
            AddAndApplyEvent(new DeveloperNameUpdatedEvent(Id, oldName, newName));
        }
        
        if (emailChanged)
        {
            AddAndApplyEvent(new DeveloperEmailUpdatedEvent(Id, oldEmail?.Value ?? string.Empty, newEmail.Value));
        }
        
        if (experienceLevelChanged)
        {
            AddAndApplyEvent(new DeveloperExperienceLevelUpdatedEvent(Id, oldExperienceLevel, experienceLevel));
        }
        
        if (bioChanged)
        {
            AddAndApplyEvent(new DeveloperBioUpdatedEvent(Id, oldBio, bio));
        }
    }
    
    /// <summary>
    /// Adds a new skill to the developer's skill collection
    /// </summary>
    /// <param name="skillName">The name of the skill</param>
    /// <param name="category">The skill category</param>
    /// <param name="proficiencyLevel">The developer's proficiency level in this skill</param>
    /// <param name="description">Optional description of the skill expertise</param>
    /// <param name="yearsOfExperience">Years of experience with this skill</param>
    /// <param name="isEndorsed">Whether the skill has been endorsed by others</param>
    /// <exception cref="BusinessRuleException">Thrown when business rules are violated</exception>
    /// <exception cref="DomainValidationException">Thrown when domain rules are violated</exception>
    public void AddSkill(
        string skillName,
        SkillCategory category,
        SkillLevel proficiencyLevel,
        string? description = null,
        int yearsOfExperience = 0,
        bool isEndorsed = false)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(skillName))
        {
            throw new BusinessRuleException("Skill name cannot be empty");
        }
        
        // Check for duplicate skills
        if (_skills.Any(s => s.Name.Equals(skillName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new BusinessRuleException($"Developer already has a skill named '{skillName}'");
        }
        
        // Create new skill
        var developerSkill = new DeveloperSkill(
            skillName,
            category,
            proficiencyLevel,
            description,
            yearsOfExperience,
            isEndorsed);
        
        _skills.Add(developerSkill);
        
        AddAndApplyEvent(new SkillAddedToDeveloperEvent(
            Id,
            Name,
            developerSkill.Id,
            skillName,
            category,
            proficiencyLevel));
    }
    
    /// <summary>
    /// Removes a skill from the developer's skill collection
    /// </summary>
    /// <param name="skillId">The ID of the skill to remove</param>
    /// <exception cref="EntityNotFoundException">Thrown when the skill is not found</exception>
    public void RemoveSkill(Guid skillId)
    {
        var skill = _skills.FirstOrDefault(s => s.Id == skillId);
        if (skill == null)
        {
            throw new EntityNotFoundException($"Skill with ID '{skillId}' not found for developer '{Name}'");
        }
        
        _skills.Remove(skill);
        
        AddAndApplyEvent(new SkillRemovedFromDeveloperEvent(
            Id,
            Name,
            skill.Id,
            skill.Name));
    }
    
    /// <summary>
    /// Updates an existing skill in the developer's collection
    /// </summary>
    /// <param name="skillId">The ID of the skill to update</param>
    /// <param name="proficiencyLevel">The new proficiency level</param>
    /// <param name="description">The new description</param>
    /// <param name="yearsOfExperience">The new years of experience</param>
    /// <param name="isEndorsed">The new endorsement status</param>
    /// <exception cref="EntityNotFoundException">Thrown when the skill is not found</exception>
    public void UpdateSkill(
        Guid skillId,
        SkillLevel proficiencyLevel,
        string? description = null,
        int yearsOfExperience = 0,
        bool isEndorsed = false)
    {
        var skill = _skills.FirstOrDefault(s => s.Id == skillId);
        if (skill == null)
        {
            throw new EntityNotFoundException($"Skill with ID '{skillId}' not found for developer '{Name}'");
        }
        
        // Store old values for events
        var oldProficiencyLevel = skill.ProficiencyLevel;
        var oldDescription = skill.Description;
        var oldYearsOfExperience = skill.YearsOfExperience;
        var oldIsEndorsed = skill.IsEndorsed;
        
        // Update the skill
        skill.UpdateProficiency(proficiencyLevel, description, yearsOfExperience, isEndorsed);
        
        // Generate event for skill update
        AddAndApplyEvent(new DeveloperSkillUpdatedEvent(
            Id,
            Name,
            skill.Id,
            skill.Name,
            oldProficiencyLevel,
            proficiencyLevel,
            oldYearsOfExperience,
            yearsOfExperience,
            oldIsEndorsed,
            isEndorsed));
    }
    
    /// <summary>
    /// Endorses a skill, marking it as validated by peers or employers
    /// </summary>
    /// <param name="skillId">The ID of the skill to endorse</param>
    /// <exception cref="EntityNotFoundException">Thrown when the skill is not found</exception>
    /// <exception cref="BusinessRuleException">Thrown when the skill is already endorsed</exception>
    public void EndorseSkill(Guid skillId)
    {
        var skill = _skills.FirstOrDefault(s => s.Id == skillId);
        if (skill == null)
        {
            throw new EntityNotFoundException($"Skill with ID '{skillId}' not found for developer '{Name}'");
        }
        
        if (skill.IsEndorsed)
        {
            throw new BusinessRuleException($"Skill '{skill.Name}' is already endorsed");
        }
        
        skill.Endorse();
        
        AddAndApplyEvent(new DeveloperSkillEndorsedEvent(
            Id,
            Name,
            skill.Id,
            skill.Name));
    }
    
    /// <summary>
    /// Gets skills by category
    /// </summary>
    /// <param name="category">The skill category to filter by</param>
    /// <returns>Collection of skills in the specified category</returns>
    public IEnumerable<DeveloperSkill> GetSkillsByCategory(SkillCategory category)
    {
        return _skills.Where(s => s.Category == category);
    }
    
    /// <summary>
    /// Gets skills by proficiency level
    /// </summary>
    /// <param name="level">The proficiency level to filter by</param>
    /// <returns>Collection of skills at the specified proficiency level</returns>
    public IEnumerable<DeveloperSkill> GetSkillsByProficiency(SkillLevel level)
    {
        return _skills.Where(s => s.ProficiencyLevel == level);
    }
    
    /// <summary>
    /// Gets endorsed skills
    /// </summary>
    /// <returns>Collection of endorsed skills</returns>
    public IEnumerable<DeveloperSkill> GetEndorsedSkills()
    {
        return _skills.Where(s => s.IsEndorsed);
    }
    
    /// <summary>
    /// Gets the total number of skills
    /// </summary>
    /// <returns>The count of skills</returns>
    public int GetSkillCount()
    {
        return _skills.Count;
    }
    
    /// <summary>
    /// Checks if the developer has a specific skill
    /// </summary>
    /// <param name="skillName">The name of the skill to check</param>
    /// <returns>True if the developer has the skill, false otherwise</returns>
    public bool HasSkill(string skillName)
    {
        return _skills.Any(s => s.Name.Equals(skillName, StringComparison.OrdinalIgnoreCase));
    }
    
    /// <summary>
    /// Updates the state of this aggregate root with a domain event
    /// </summary>
    /// <param name="domainEvent">The domain event that modifies the aggregate state</param>
    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case DeveloperCreatedEvent created:
                // For event sourcing, set the ID from the event
                if (Id == Guid.Empty || Id != created.DeveloperId)
                {
                    var idProperty = typeof(BaseEntity).GetProperty(nameof(Id));
                    idProperty?.SetValue(this, created.DeveloperId);
                }
                Name = created.Name;
                Email = new Email(created.Email);
                ExperienceLevel = created.ExperienceLevel;
                break;
                
            case DeveloperNameUpdatedEvent nameUpdated:
                Name = nameUpdated.NewName;
                break;
                
            case DeveloperEmailUpdatedEvent emailUpdated:
                Email = new Email(emailUpdated.NewEmail);
                break;
                
            case DeveloperExperienceLevelUpdatedEvent experienceUpdated:
                ExperienceLevel = experienceUpdated.NewLevel;
                break;
                
            case DeveloperBioUpdatedEvent bioUpdated:
                Bio = bioUpdated.NewBio;
                break;
                
            case SkillAddedToDeveloperEvent skillAdded:
                // Event sourcing: skill should already be in collection
                // This is for side effects only
                break;
                
            case SkillRemovedFromDeveloperEvent skillRemoved:
                // Event sourcing: skill should already be removed
                // This is for side effects only
                break;
                
            case DeveloperSkillUpdatedEvent skillUpdated:
                // Event sourcing: skill should already be updated
                // This is for side effects only
                break;
                
            case DeveloperSkillEndorsedEvent skillEndorsed:
                // Event sourcing: skill should already be endorsed
                // This is for side effects only
                break;
        }
    }
    
    /// <summary>
    /// Validates that the Developer entity satisfies all invariants
    /// </summary>
    /// <returns>A collection of error messages if any invariants are violated</returns>
    protected override IEnumerable<string> CheckInvariants()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            yield return "Developer name cannot be empty";
        }
        
        if (Name?.Length > 200)
        {
            yield return "Developer name cannot exceed 200 characters";
        }
        
        if (Email == null)
        {
            yield return "Developer email cannot be null";
        }
        
        if (Bio?.Length > 2000)
        {
            yield return "Developer bio cannot exceed 2000 characters";
        }
        
        // Validate unique skill names within the collection
        var skillNames = _skills.Select(s => s.Name.ToLowerInvariant()).ToList();
        if (skillNames.Count != skillNames.Distinct().Count())
        {
            yield return "Developer cannot have duplicate skills";
        }
        
        // Validate that all skills are valid
        foreach (var skill in _skills)
        {
            foreach (var error in skill.ValidateInvariants())
            {
                yield return $"Skill '{skill.Name}': {error}";
            }
        }
    }
}
