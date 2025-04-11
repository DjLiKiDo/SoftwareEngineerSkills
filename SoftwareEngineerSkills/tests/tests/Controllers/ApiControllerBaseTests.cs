using System.Net;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SoftwareEngineerSkills.API.Controllers;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Tests.Controllers;

/// <summary>
/// Unit tests for the <see cref="ApiControllerBase"/> class
/// </summary>
public class ApiControllerBaseTests
{
    /// <summary>
    /// Test class that inherits from ApiControllerBase for testing purposes
    /// </summary>
    private class TestController : ApiControllerBase
    {
        /// <summary>
        /// Exposes the protected HandleResult method for testing
        /// </summary>
        public IActionResult TestHandleResult<T>(Result<T> result)
        {
            return HandleResult(result);
        }
    }

    /// <summary>
    /// Tests that HandleResult returns an Ok result when the Result is successful with a non-default value
    /// </summary>
    [Fact]
    public void HandleResult_WhenSuccessWithNonDefaultValue_ReturnsOkResult()
    {
        // Arrange
        var controller = CreateControllerWithHttpContext();
        var testData = "Test Data";
        var result = Result<string>.Success(testData);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        actionResult.Should().BeOfType<OkObjectResult>();
        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().Be(testData);
    }

    /// <summary>
    /// Tests that HandleResult returns a NotFound result when the Result is successful with a default value
    /// </summary>
    [Fact]
    public void HandleResult_WhenSuccessWithDefaultValue_ReturnsNotFoundResult()
    {
        // Arrange
        var controller = CreateControllerWithHttpContext();
        var result = Result<string>.Success(default);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        actionResult.Should().BeOfType<NotFoundResult>();
        var notFoundResult = actionResult as NotFoundResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that HandleResult returns a BadRequest result when the Result is a failure
    /// </summary>
    [Fact]
    public void HandleResult_WhenFailure_ReturnsBadRequestResult()
    {
        // Arrange
        var controller = CreateControllerWithHttpContext();
        var errorMessage = "Test Error";
        var result = Result<string>.Failure(errorMessage);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        actionResult.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = actionResult as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        
        var problemDetails = badRequestResult.Value as ProblemDetails;
        problemDetails.Should().NotBeNull();
        problemDetails!.Title.Should().Be("Application Error");
        problemDetails.Detail.Should().Be(errorMessage);
        problemDetails.Status.Should().Be(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Tests that HandleResult works with numeric types (int) when successful with non-default value
    /// </summary>
    [Fact]
    public void HandleResult_WithIntType_WhenSuccessWithNonZeroValue_ReturnsOkResult()
    {
        // Arrange
        var controller = CreateControllerWithHttpContext();
        var testData = 42;
        var result = Result<int>.Success(testData);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        actionResult.Should().BeOfType<OkObjectResult>();
        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(testData);
    }

    /// <summary>
    /// Tests that HandleResult works with numeric types (int) when successful with default value (0)
    /// </summary>
    [Fact]
    public void HandleResult_WithIntType_WhenSuccessWithZeroValue_ReturnsNotFoundResult()
    {
        // Arrange
        var controller = CreateControllerWithHttpContext();
        var result = Result<int>.Success(0); // 0 is default for int

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        actionResult.Should().BeOfType<NotFoundResult>();
    }

    /// <summary>
    /// Tests that HandleResult works with reference types (class) when successful with non-null value
    /// </summary>
    [Fact]
    public void HandleResult_WithReferenceType_WhenSuccessWithNonNullValue_ReturnsOkResult()
    {
        // Arrange
        var controller = CreateControllerWithHttpContext();
        var testData = new TestClass { Id = 1, Name = "Test" };
        var result = Result<TestClass>.Success(testData);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        actionResult.Should().BeOfType<OkObjectResult>();
        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(testData);
    }

    /// <summary>
    /// Tests that HandleResult works with reference types (class) when successful with null value
    /// </summary>
    [Fact]
    public void HandleResult_WithReferenceType_WhenSuccessWithNullValue_ReturnsNotFoundResult()
    {
        // Arrange
        var controller = CreateControllerWithHttpContext();
        var result = Result<TestClass>.Success(null);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        actionResult.Should().BeOfType<NotFoundResult>();
    }

    /// <summary>
    /// Helper method to create a TestController with an HttpContext that has RequestServices
    /// </summary>
    private TestController CreateControllerWithHttpContext()
    {
        var mediatorMock = new Mock<ISender>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ISender))).Returns(mediatorMock.Object);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProviderMock.Object
        };

        var controller = new TestController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        return controller;
    }

    /// <summary>
    /// Test class for testing reference type handling
    /// </summary>
    private class TestClass
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}