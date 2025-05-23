using SoftwareEngineerSkills.Domain.Entities.Skills;

namespace SoftwareEngineerSkills.Domain.Abstractions.Persistence;

/// <summary>
/// Repository interface for Skill entities
/// </summary>
public interface ISkillRepository : IRepository<Skill>
{
    /// <summary>
    /// Finds a skill by its name
    /// </summary>
    /// <param name="name">The name of the skill to find</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>The skill if found, otherwise null</returns>
    Task<Skill?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets skills by category
    /// </summary>
    /// <param name="category">The category to filter by</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>The skills in the specified category</returns>
    Task<IEnumerable<Skill>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets skills by difficulty level
    /// </summary>
    /// <param name="level">The difficulty level to filter by</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>The skills with the specified difficulty level</returns>
    Task<IEnumerable<Skill>> GetByDifficultyLevelAsync(int level, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets skills that are in demand
    /// </summary>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>The skills that are in demand</returns>
    Task<IEnumerable<Skill>> GetInDemandSkillsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if a skill with the given name already exists
    /// </summary>
    /// <param name="name">The name to check</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>True if a skill with the name exists; otherwise, false</returns>
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a paged list of skills
    /// </summary>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>A paginated list of skills</returns>
    Task<(IEnumerable<Skill> Skills, int TotalCount)> GetPagedSkillsAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
