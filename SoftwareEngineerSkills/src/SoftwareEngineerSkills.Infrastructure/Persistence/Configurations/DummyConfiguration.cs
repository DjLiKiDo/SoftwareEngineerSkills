using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoftwareEngineerSkills.Domain.Entities;

namespace SoftwareEngineerSkills.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Dummy entity
/// </summary>
public class DummyConfiguration : IEntityTypeConfiguration<Dummy>
{
    /// <summary>
    /// Configures the entity
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<Dummy> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .HasMaxLength(100);

        builder.Property(d => d.Description)
            .HasMaxLength(500);

        builder.Property(d => d.Priority);

        builder.Property(d => d.IsActive);

        builder.Property(d => d.CreatedAt);

        builder.Property(d => d.UpdatedAt);

        // Ignore domain events collection as it's not mapped to the database
        builder.Ignore(e => e.DomainEvents);
    }
}
