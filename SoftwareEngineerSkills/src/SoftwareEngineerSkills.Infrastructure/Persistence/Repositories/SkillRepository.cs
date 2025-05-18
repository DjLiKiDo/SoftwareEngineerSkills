using Microsoft.EntityFrameworkCore;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Entities.Skills;
using SoftwareEngineerSkills.Domain.Enums;

namespace SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of the skill repository
/// </summary>
internal class SkillRepository : EfRepository<Skill>, ISkillRepository
{
    /// <summary>
    /// Creates a new instance of the SkillRepository class
    /// </summary>
    /// <param name="context">The database context</param>
    public SkillRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Skill?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Skill>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        if (Enum.TryParse<SkillCategory>(category, true, out var skillCategory))
        {
            return await _dbSet
                .Where(s => s.Category == skillCategory)
                .ToListAsync(cancellationToken);
        }

        return Enumerable.Empty<Skill>();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Skill>> GetByDifficultyLevelAsync(int level, CancellationToken cancellationToken = default)
    {
        if (Enum.IsDefined(typeof(SkillLevel), level))
        {
            var skillLevel = (SkillLevel)level;
            return await _dbSet
                .Where(s => s.DifficultyLevel == skillLevel)
                .ToListAsync(cancellationToken);
        }

        return Enumerable.Empty<Skill>();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Skill>> GetInDemandSkillsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.IsInDemand)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(s => s.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Skill> Skills, int TotalCount)> GetPagedSkillsAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await _dbSet.CountAsync(cancellationToken);
        
        var skills = await _dbSet
            .OrderBy(s => s.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (skills, totalCount);
    }
}
