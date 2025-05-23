namespace SoftwareEngineerSkills.Domain.DomainServices.Interfaces;

/// <summary>
/// Interface for accessing current user information from infrastructure services.
/// This is used primarily for unit testing the CurrentUserService.
/// </summary>
public interface ICurrentUserAccessor : ICurrentUserService
{
    // This interface extends ICurrentUserService to allow mocking both in tests
}
