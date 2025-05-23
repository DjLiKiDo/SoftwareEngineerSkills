using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Infrastructure.Services.Caching;
using System.Text.Json;

namespace SoftwareEngineerSkills.Infrastructure.UnitTests.Services.Caching;

public class DistributedCacheServiceTests
{
    private readonly Mock<IDistributedCache> _distributedCacheMock;
    private readonly Mock<IOptions<CacheSettings>> _optionsMock;
    private readonly Mock<ILogger<DistributedCacheService>> _loggerMock;
    private readonly CacheSettings _cacheSettings;
    private readonly DistributedCacheService _cacheService;
    private readonly JsonSerializerOptions _jsonOptions;

    public DistributedCacheServiceTests()
    {
        _distributedCacheMock = new Mock<IDistributedCache>();
        _loggerMock = new Mock<ILogger<DistributedCacheService>>();
        
        _cacheSettings = new CacheSettings
        {
            DefaultExpirationMinutes = 30,
            EnableSlidingExpiration = true
        };
        
        _optionsMock = new Mock<IOptions<CacheSettings>>();
        _optionsMock.Setup(o => o.Value).Returns(_cacheSettings);
        
        _cacheService = new DistributedCacheService(_distributedCacheMock.Object, _optionsMock.Object, _loggerMock.Object);
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }
    
    [Fact]
    public async Task GetAsync_CacheHit_ShouldReturnDeserializedValue()
    {
        // Arrange
        var cacheKey = "test-key";
        var testObject = new TestObject { Id = 1, Name = "Test" };
        var serializedValue = JsonSerializer.Serialize(testObject, _jsonOptions);
        
        _distributedCacheMock
            .Setup(m => m.GetStringAsync(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serializedValue);

        // Act
        var result = await _cacheService.GetAsync<TestObject>(cacheKey);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(testObject);
        
        _distributedCacheMock.Verify(
            m => m.GetStringAsync(cacheKey, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task GetAsync_CacheMiss_ShouldReturnNull()
    {
        // Arrange
        var cacheKey = "test-key";
        
        _distributedCacheMock
            .Setup(m => m.GetStringAsync(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        // Act
        var result = await _cacheService.GetAsync<TestObject>(cacheKey);

        // Assert
        result.Should().BeNull();
        
        _distributedCacheMock.Verify(
            m => m.GetStringAsync(cacheKey, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task GetAsync_ExceptionThrown_ShouldCatchAndReturnNull()
    {
        // Arrange
        var cacheKey = "test-key";
        
        _distributedCacheMock
            .Setup(m => m.GetStringAsync(cacheKey, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Cache error"));

        // Act
        var result = await _cacheService.GetAsync<TestObject>(cacheKey);

        // Assert
        result.Should().BeNull();
        
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task SetAsync_ValidValue_ShouldSerializeAndSetValue()
    {
        // Arrange
        var cacheKey = "test-key";
        var valueToCache = new TestObject { Id = 1, Name = "Test" };

        // Act
        await _cacheService.SetAsync(cacheKey, valueToCache);

        // Assert
        _distributedCacheMock.Verify(
            m => m.SetStringAsync(
                cacheKey, 
                It.IsAny<string>(), // We can't easily verify the serialized value content
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task SetAsync_WithCustomExpiration_ShouldUseProvidedExpiration()
    {
        // Arrange
        var cacheKey = "test-key";
        var valueToCache = new TestObject { Id = 1, Name = "Test" };
        var customExpirationMinutes = 15;

        // Act
        await _cacheService.SetAsync(cacheKey, valueToCache, customExpirationMinutes);

        // Assert
        _distributedCacheMock.Verify(
            m => m.SetStringAsync(
                cacheKey, 
                It.IsAny<string>(),
                It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(customExpirationMinutes)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task SetAsync_ExceptionThrown_ShouldCatchAndLog()
    {
        // Arrange
        var cacheKey = "test-key";
        var valueToCache = new TestObject { Id = 1, Name = "Test" };
        
        _distributedCacheMock
            .Setup(m => m.SetStringAsync(
                cacheKey, 
                It.IsAny<string>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Cache error"));

        // Act
        await _cacheService.SetAsync(cacheKey, valueToCache);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }
    
    [Fact]
    public async Task RemoveAsync_ValidKey_ShouldRemoveCacheEntry()
    {
        // Arrange
        var cacheKey = "test-key";

        // Act
        await _cacheService.RemoveAsync(cacheKey);

        // Assert
        _distributedCacheMock.Verify(
            m => m.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task RemoveAsync_ExceptionThrown_ShouldCatchAndLog()
    {
        // Arrange
        var cacheKey = "test-key";
        
        _distributedCacheMock
            .Setup(m => m.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Cache error"));

        // Act
        await _cacheService.RemoveAsync(cacheKey);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }

    // Helper class for testing
    private class TestObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
