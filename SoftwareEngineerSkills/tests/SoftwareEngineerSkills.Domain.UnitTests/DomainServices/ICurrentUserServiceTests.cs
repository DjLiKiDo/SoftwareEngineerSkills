using FluentAssertions;
using Moq;
using SoftwareEngineerSkills.Domain.DomainServices.Interfaces;
using System;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.DomainServices;

public class ICurrentUserServiceTests
{
    [Fact]
    public void ICurrentUserService_ShouldHaveExpectedMembers()
    {
        // Arrange
        var type = typeof(ICurrentUserService);
        
        // Act & Assert
        type.Should().BeInterface();
        
        // Check UserId property
        var userIdProperty = type.GetProperty("UserId");
        userIdProperty.Should().NotBeNull();
        userIdProperty.PropertyType.Should().Be(typeof(string).MakeNullable());
        userIdProperty.CanRead.Should().BeTrue();
        userIdProperty.CanWrite.Should().BeFalse(); // Should only have a getter
        
        // Check UserName property
        var userNameProperty = type.GetProperty("UserName");
        userNameProperty.Should().NotBeNull();
        userNameProperty.PropertyType.Should().Be(typeof(string).MakeNullable());
        userNameProperty.CanRead.Should().BeTrue();
        userNameProperty.CanWrite.Should().BeFalse(); // Should only have a getter
        
        // Check IsAuthenticated property
        var isAuthenticatedProperty = type.GetProperty("IsAuthenticated");
        isAuthenticatedProperty.Should().NotBeNull();
        isAuthenticatedProperty.PropertyType.Should().Be(typeof(bool));
        isAuthenticatedProperty.CanRead.Should().BeTrue();
        isAuthenticatedProperty.CanWrite.Should().BeFalse(); // Should only have a getter
    }
    
    [Fact]
    public void ICurrentUserService_Implementation_AuthenticatedUser_ShouldWork()
    {
        // Arrange
        var mockUserService = new Mock<ICurrentUserService>();
        var userId = "user-123";
        var userName = "John Doe";
        
        mockUserService.Setup(us => us.UserId).Returns(userId);
        mockUserService.Setup(us => us.UserName).Returns(userName);
        mockUserService.Setup(us => us.IsAuthenticated).Returns(true);
        
        // Act
        var resultUserId = mockUserService.Object.UserId;
        var resultUserName = mockUserService.Object.UserName;
        var isAuthenticated = mockUserService.Object.IsAuthenticated;
        
        // Assert
        resultUserId.Should().Be(userId);
        resultUserName.Should().Be(userName);
        isAuthenticated.Should().BeTrue();
        
        mockUserService.Verify(us => us.UserId, Times.Once());
        mockUserService.Verify(us => us.UserName, Times.Once());
        mockUserService.Verify(us => us.IsAuthenticated, Times.Once());
    }
    
    [Fact]
    public void ICurrentUserService_Implementation_UnauthenticatedUser_ShouldWork()
    {
        // Arrange
        var mockUserService = new Mock<ICurrentUserService>();
        
        mockUserService.Setup(us => us.UserId).Returns((string)null);
        mockUserService.Setup(us => us.UserName).Returns((string)null);
        mockUserService.Setup(us => us.IsAuthenticated).Returns(false);
        
        // Act
        var resultUserId = mockUserService.Object.UserId;
        var resultUserName = mockUserService.Object.UserName;
        var isAuthenticated = mockUserService.Object.IsAuthenticated;
        
        // Assert
        resultUserId.Should().BeNull();
        resultUserName.Should().BeNull();
        isAuthenticated.Should().BeFalse();
        
        mockUserService.Verify(us => us.UserId, Times.Once());
        mockUserService.Verify(us => us.UserName, Times.Once());
        mockUserService.Verify(us => us.IsAuthenticated, Times.Once());
    }
    
    [Fact]
    public void ICurrentUserAccessor_ShouldExtendICurrentUserService()
    {
        // Arrange
        var accessorType = typeof(ICurrentUserAccessor);
        var serviceType = typeof(ICurrentUserService);
        
        // Act & Assert
        accessorType.Should().BeInterface();
        accessorType.Should().BeAssignableTo(serviceType);
        serviceType.IsAssignableFrom(accessorType).Should().BeTrue();
    }
}
