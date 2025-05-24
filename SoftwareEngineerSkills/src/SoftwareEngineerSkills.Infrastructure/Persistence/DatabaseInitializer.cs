using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Domain.Entities.Skills;
using SoftwareEngineerSkills.Domain.Enums;

namespace SoftwareEngineerSkills.Infrastructure.Persistence;

/// <summary>
/// Initializes the database with seed data
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Seeds the database with initial data
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // Apply migrations if not using InMemory database
            if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                await context.Database.MigrateAsync();
                logger.LogInformation("Applied database migrations");
            }

            await SeedSkillsAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private static async Task SeedSkillsAsync(ApplicationDbContext context, ILogger logger)
    {
        // Only seed if the Skills table is empty
        if (await context.Skills.AnyAsync())
        {
            logger.LogInformation("Skills table already contains data, skipping seeding");
            return;
        }

        logger.LogInformation("Seeding Skills table...");

        var skills = new List<Skill>
        {
            new Skill(
                "C# Programming", 
                SkillCategory.ProgrammingLanguage, 
                "Object-oriented programming language developed by Microsoft", 
                SkillLevel.Intermediate, 
                isInDemand: true),
                
            new Skill(
                "ASP.NET Core",
                SkillCategory.Framework, 
                "Cross-platform framework for building web applications", 
                SkillLevel.Advanced, 
                isInDemand: true),
                
            new Skill(
                "Entity Framework Core", 
                SkillCategory.Framework, 
                "Object-relational mapper for .NET", 
                SkillLevel.Intermediate, 
                isInDemand: true),
                 new Skill(
                "Clean Architecture", 
                SkillCategory.Architecture,
                "Software design philosophy emphasizing separation of concerns", 
                SkillLevel.Advanced,
                isInDemand: true),
                
            new Skill(
                "Domain-Driven Design", 
                SkillCategory.Architecture,
                "Approach to software development focusing on the core domain", 
                SkillLevel.Advanced, 
                isInDemand: true),
                
            new Skill(
                "SQL", 
                SkillCategory.Database,
                "Standard language for managing and querying relational databases", 
                SkillLevel.Intermediate, 
                isInDemand: true),
                
            new Skill(
                "Git", 
                SkillCategory.Tool,
                "Distributed version control system", 
                SkillLevel.Beginner, 
                isInDemand: true),
                
            new Skill(
                "Docker", 
                SkillCategory.DevOps,
                "Platform for containerizing applications", 
                SkillLevel.Intermediate, 
                isInDemand: true),
                
            new Skill(
                "Azure", 
                SkillCategory.Cloud,
                "Microsoft's cloud computing platform", 
                SkillLevel.Intermediate, 
                isInDemand: true),
                
            new Skill(
                "Unit Testing", 
                SkillCategory.Testing,
                "Software testing method where individual units are tested", 
                SkillLevel.Intermediate, 
                isInDemand: true)
        };

        await context.Skills.AddRangeAsync(skills);
        await context.SaveChangesAsync();
        
        logger.LogInformation("Seeded {SkillCount} skills", skills.Count);
    }
}
