using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Domain.DomainServices.Interfaces;
using System.Security.Claims;

namespace SoftwareEngineerSkills.Infrastructure.Services.User;

/// <summary>
/// Implementation of the ICurrentUserService that gets user info from the HttpContext
/// </summary>
public class CurrentUserService : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly ClaimsPrincipal? _user;
    private readonly ILogger<CurrentUserService>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserService"/> class for production use.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserService"/> class for testing.
    /// </summary>
    /// <param name="user">The claims principal to use for testing.</param>
    /// <param name="logger">Logger for the service.</param>
    public CurrentUserService(ClaimsPrincipal? user, ILogger<CurrentUserService>? logger = null)
    {
        _user = user;
        _logger = logger;
    }

    /// <inheritdoc/>
    public string? UserId
    {
        get
        {
            if (_user != null)
                return _user.FindFirstValue(ClaimTypes.NameIdentifier);
                
            return _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }

    /// <inheritdoc/>
    public string? UserName
    {
        get
        {
            if (_user != null)
            {
                var name = _user.FindFirstValue(ClaimTypes.Name);
                return string.IsNullOrEmpty(name) ? UserId : name;
            }
                
            var httpName = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
            var httpId = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrEmpty(httpName) ? httpId : httpName;
        }
    }

    /// <inheritdoc/>
    public bool IsAuthenticated
    {
        get
        {
            if (_user != null)
                return _user.Identity?.IsAuthenticated ?? false;
                
            return _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }
    }
}
