using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Infrastructure.Services.User;
using System.Security.Claims;

namespace SoftwareEngineerSkills.Infrastructure.UnitTests.Services.User;

public class CurrentUserServiceTests
{
    private readonly Mock<ClaimsPrincipal> _userMock;
    private readonly Mock<ILogger<CurrentUserService>> _loggerMock;
    
    public CurrentUserServiceTests()
    {
        _userMock = new Mock<ClaimsPrincipal>();
        _loggerMock = new Mock<ILogger<CurrentUserService>>();
    }

    [Fact]
    public void Constructor_WithAuthenticatedUser_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var userId = "user-123";
        var userName = "testuser@example.com";
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName)
        };
        
        _userMock.Setup(u => u.Claims).Returns(claims);
        _userMock.Setup(u => u.Identity!.IsAuthenticated).Returns(true);
        
        // Act
        var currentUserService = new CurrentUserService(_userMock.Object, _loggerMock.Object);
        
        // Assert
        currentUserService.UserId.Should().Be(userId);
        currentUserService.UserName.Should().Be(userName);
        currentUserService.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithoutNameClaim_ShouldUseIdAsName()
    {
        // Arrange
        var userId = "user-123";
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
        
        _userMock.Setup(u => u.Claims).Returns(claims);
        _userMock.Setup(u => u.Identity!.IsAuthenticated).Returns(true);
        
        // Act
        var currentUserService = new CurrentUserService(_userMock.Object, _loggerMock.Object);
        
        // Assert
        currentUserService.UserId.Should().Be(userId);
        currentUserService.UserName.Should().Be(userId); // Should use ID as username
        currentUserService.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithoutIdClaim_ShouldSetNullUserId()
    {
        // Arrange
        var userName = "testuser@example.com";
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userName)
        };
        
        _userMock.Setup(u => u.Claims).Returns(claims);
        _userMock.Setup(u => u.Identity!.IsAuthenticated).Returns(true);
        
        // Act
        var currentUserService = new CurrentUserService(_userMock.Object, _loggerMock.Object);
        
        // Assert
        currentUserService.UserId.Should().BeNull();
        currentUserService.UserName.Should().Be(userName);
        currentUserService.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithUnauthenticatedUser_ShouldSetIsAuthenticatedFalse()
    {
        // Arrange
        _userMock.Setup(u => u.Identity!.IsAuthenticated).Returns(false);
        
        // Act
        var currentUserService = new CurrentUserService(_userMock.Object, _loggerMock.Object);
        
        // Assert
        currentUserService.UserId.Should().BeNull();
        currentUserService.UserName.Should().BeNull();
        currentUserService.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithNullIdentity_ShouldHandleGracefully()
    {
        // Arrange
        _userMock.Setup(u => u.Identity).Returns((System.Security.Principal.IIdentity?)null);
        
        // Act
        var currentUserService = new CurrentUserService(_userMock.Object, _loggerMock.Object);
        
        // Assert
        currentUserService.UserId.Should().BeNull();
        currentUserService.UserName.Should().BeNull();
        currentUserService.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithNullUser_ShouldHandleGracefully()
    {
        // Act
        var currentUserService = new CurrentUserService(null!, _loggerMock.Object);
        
        // Assert
        currentUserService.UserId.Should().BeNull();
        currentUserService.UserName.Should().BeNull();
        currentUserService.IsAuthenticated.Should().BeFalse();
    }
}
