using SoftwareEngineerSkills.Domain.Entities;

namespace SoftwareEngineerSkills.Domain.Abstractions.Services;

/// <summary>
/// Domain service for performing complex operations on Dummy entities
/// </summary>
public interface IDummyDomainService
{
    /// <summary>
    /// Verifies if a dummy entity meets domain-specific business rules
    /// </summary>
    /// <param name="dummy">The dummy entity to validate</param>
    /// <returns>True if the entity is valid, false otherwise</returns>
    bool ValidateDummyBusinessRules(Dummy dummy);
    
    /// <summary>
    /// Performs a complex domain operation involving multiple dummy entities
    /// </summary>
    /// <param name="dummies">The collection of dummy entities</param>
    /// <exception cref="DomainException">Thrown when the operation fails due to business rule violations</exception>
    Task ProcessDummiesAsync(IEnumerable<Dummy> dummies);
}
