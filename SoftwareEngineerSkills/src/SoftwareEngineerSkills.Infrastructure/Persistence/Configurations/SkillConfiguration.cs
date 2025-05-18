using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoftwareEngineerSkills.Domain.Entities.Skills;

namespace SoftwareEngineerSkills.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for the Skill entity
/// </summary>
public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    /// <summary>
    /// Configures the Skill entity
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("Skills");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Category)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(s => s.DifficultyLevel)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(s => s.IsInDemand)
            .IsRequired()
            .HasDefaultValue(false);

        // Ignore domain events collection as it's not part of the persistence model
        builder.Ignore(s => s.DomainEvents);

        // Create a unique index on Name
        builder.HasIndex(s => s.Name)
            .IsUnique();

        // Create an index on Category for faster lookups
        builder.HasIndex(s => s.Category);

        // Create an index on DifficultyLevel for faster lookups
        builder.HasIndex(s => s.DifficultyLevel);
    }
}
