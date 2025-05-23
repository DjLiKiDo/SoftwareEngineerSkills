using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.API.Middleware;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace SoftwareEngineerSkills.API.UnitTests.Middleware;

public class GlobalExceptionHandlerTests
{
    private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
    private readonly GlobalExceptionHandler _handler;

    public GlobalExceptionHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
        _handler = new GlobalExceptionHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task TryHandleAsync_ValidationException_ShouldReturn400BadRequest()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var exception = new ValidationException("Invalid data");

        // Act
        var result = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        
        // Verify the response body contains a ProblemDetails object
        var responseBody = await GetResponseBodyContent(httpContext);
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody);
        
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(StatusCodes.Status400BadRequest);
        problemDetails.Title.Should().Be("Validation Error");
        problemDetails.Detail.Should().Be("Invalid data");

        // Verify the logger was called
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task TryHandleAsync_UnauthorizedAccessException_ShouldReturn401Unauthorized()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var exception = new UnauthorizedAccessException("Access denied");

        // Act
        var result = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        
        // Verify the response body contains a ProblemDetails object
        var responseBody = await GetResponseBodyContent(httpContext);
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody);
        
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(StatusCodes.Status401Unauthorized);
        problemDetails.Title.Should().Be("Unauthorized Access");
        problemDetails.Detail.Should().Be("Access denied");
    }

    [Fact]
    public async Task TryHandleAsync_GenericException_ShouldReturn500InternalServerError()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var exception = new Exception("Something went wrong");

        // Act
        var result = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        
        // Verify the response body contains a ProblemDetails object
        var responseBody = await GetResponseBodyContent(httpContext);
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody);
        
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(StatusCodes.Status500InternalServerError);
        problemDetails.Title.Should().Be("An unexpected error occurred");
        problemDetails.Detail.Should().Be("Something went wrong");
    }

    private HttpContext CreateMockHttpContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        httpContext.Request.Path = "/api/test";
        return httpContext;
    }

    private async Task<string> GetResponseBodyContent(HttpContext httpContext)
    {
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(httpContext.Response.Body, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
}
