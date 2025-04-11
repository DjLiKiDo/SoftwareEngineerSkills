using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareEngineerSkills.API.Controllers;
using SoftwareEngineerSkills.Application.Features.Dummy.Commands.CreateDummy;
using SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetAllDummies;
using SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetDummyById;
using SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetDummyConfiguration;
using Xunit;

namespace SoftwareEngineerSkills.Tests.Controllers;

public class DummyControllerTests
{
    private readonly Mock<ILogger<DummyController>> _loggerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DummyController _controller;

    public DummyControllerTests()
    {
        _loggerMock = new Mock<ILogger<DummyController>>();
        _mediatorMock = new Mock<IMediator>();
        _controller = new DummyController(_loggerMock.Object)
        {
            Mediator = _mediatorMock.Object
        };
    }

    [Fact]
    public async Task GetConfigurationAsync_ReturnsOkResult_WhenSuccessful()
    {
        // Arrange
        var response = new GetDummyConfigurationResponse { /* Populate with test data */ };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetDummyConfigurationQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GetDummyConfigurationResponse>.Success(response));

        // Act
        var result = await _controller.GetConfigurationAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task GetConfigurationAsync_ReturnsBadRequest_WhenResultIsFailure()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetDummyConfigurationQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GetDummyConfigurationResponse>.Failure("Error"));

        // Act
        var result = await _controller.GetConfigurationAsync(CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error", badRequestResult.Value);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOkResult_WhenSuccessful()
    {
        // Arrange
        var response = new List<DummyDto> { new DummyDto { Id = Guid.NewGuid(), Name = "Test" } };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllDummiesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<DummyDto>>.Success(response));

        // Act
        var result = await _controller.GetAllAsync(false, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsOkResult_WhenEntityExists()
    {
        // Arrange
        var dummyId = Guid.NewGuid();
        var response = new DummyDto { Id = dummyId, Name = "Test" };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetDummyByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DummyDto>.Success(response));

        // Act
        var result = await _controller.GetByIdAsync(dummyId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedResult_WhenSuccessful()
    {
        // Arrange
        var dummyId = Guid.NewGuid();
        var command = new CreateDummyCommand { Name = "Test" };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateDummyCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(dummyId));

        // Act
        var result = await _controller.CreateAsync(command, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(dummyId, createdResult.Value);
    }
}