using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Infrastructure.Services.Caching;

namespace SoftwareEngineerSkills.Infrastructure.UnitTests.Services.Caching;

public class MemoryCacheServiceTests
{
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    private readonly Mock<IOptions<CacheSettings>> _optionsMock;
    private readonly Mock<ILogger<MemoryCacheService>> _loggerMock;
    private readonly CacheSettings _cacheSettings;
    private readonly MemoryCacheService _cacheService;

    public MemoryCacheServiceTests()
    {
        _memoryCacheMock = new Mock<IMemoryCache>();
        _loggerMock = new Mock<ILogger<MemoryCacheService>>();
        
        _cacheSettings = new CacheSettings
        {
            DefaultExpirationMinutes = 30,
            EnableSlidingExpiration = true
        };
        
        _optionsMock = new Mock<IOptions<CacheSettings>>();
        _optionsMock.Setup(o => o.Value).Returns(_cacheSettings);
        
        _cacheService = new MemoryCacheService(_memoryCacheMock.Object, _optionsMock.Object, _loggerMock.Object);
    }
    
    [Fact]
    public async Task GetAsync_CacheHit_ShouldReturnCachedValue()
    {
        // Arrange
        var cacheKey = "test-key";
        var cachedValue = new TestObject { Id = 1, Name = "Test" };
        
        var cacheEntryMock = new Mock<ICacheEntry>();
        
        _memoryCacheMock
            .Setup(m => m.TryGetValue(cacheKey, out It.Ref<TestObject?>.IsAny))
            .Callback(new OutCallback<string, TestObject?>((string key, out TestObject? value) => value = cachedValue))
            .Returns(true);

        // Act
        var result = await _cacheService.GetAsync<TestObject>(cacheKey);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(cachedValue);
        
        _memoryCacheMock.Verify(m => m.TryGetValue(cacheKey, out It.Ref<TestObject?>.IsAny), Times.Once);
    }

    [Fact]
    public async Task GetAsync_CacheMiss_ShouldReturnNull()
    {
        // Arrange
        var cacheKey = "test-key";
        
        _memoryCacheMock
            .Setup(m => m.TryGetValue(cacheKey, out It.Ref<TestObject?>.IsAny))
            .Callback(new OutCallback<string, TestObject?>((string key, out TestObject? value) => value = null))
            .Returns(false);

        // Act
        var result = await _cacheService.GetAsync<TestObject>(cacheKey);

        // Assert
        result.Should().BeNull();
        
        _memoryCacheMock.Verify(m => m.TryGetValue(cacheKey, out It.Ref<TestObject?>.IsAny), Times.Once);
    }
    
    [Fact]
    public async Task GetAsync_ExceptionThrown_ShouldCatchAndReturnNull()
    {
        // Arrange
        var cacheKey = "test-key";
        
        _memoryCacheMock
            .Setup(m => m.TryGetValue(cacheKey, out It.Ref<TestObject?>.IsAny))
            .Throws(new InvalidOperationException("Cache error"));

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
    public async Task SetAsync_ValidValue_ShouldSetCacheEntry()
    {
        // Arrange
        var cacheKey = "test-key";
        var valueToCache = new TestObject { Id = 1, Name = "Test" };
        var cacheEntryMock = new Mock<ICacheEntry>();
        
        _memoryCacheMock
            .Setup(m => m.CreateEntry(cacheKey))
            .Returns(cacheEntryMock.Object);

        // Act
        await _cacheService.SetAsync(cacheKey, valueToCache);

        // Assert
        _memoryCacheMock.Verify(m => m.CreateEntry(cacheKey), Times.Once);
        cacheEntryMock.VerifySet(e => e.Value = valueToCache, Times.Once);
    }
    
    [Fact]
    public async Task SetAsync_WithCustomExpiration_ShouldUseProvidedExpiration()
    {
        // Arrange
        var cacheKey = "test-key";
        var valueToCache = new TestObject { Id = 1, Name = "Test" };
        var customExpirationMinutes = 15;
        
        var cacheEntryMock = new Mock<ICacheEntry>();
        
        _memoryCacheMock
            .Setup(m => m.CreateEntry(cacheKey))
            .Returns(cacheEntryMock.Object);

        // Act
        await _cacheService.SetAsync(cacheKey, valueToCache, customExpirationMinutes);

        // Assert
        _memoryCacheMock.Verify(m => m.CreateEntry(cacheKey), Times.Once);
        cacheEntryMock.VerifySet(e => e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(customExpirationMinutes), Times.Once);
        cacheEntryMock.VerifySet(e => e.Value = valueToCache, Times.Once);
    }
    
    [Fact]
    public async Task SetAsync_ExceptionThrown_ShouldCatchAndLog()
    {
        // Arrange
        var cacheKey = "test-key";
        var valueToCache = new TestObject { Id = 1, Name = "Test" };
        
        _memoryCacheMock
            .Setup(m => m.CreateEntry(cacheKey))
            .Throws(new InvalidOperationException("Cache error"));

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
        _memoryCacheMock.Verify(m => m.Remove(cacheKey), Times.Once);
    }
    
    [Fact]
    public async Task RemoveAsync_ExceptionThrown_ShouldCatchAndLog()
    {
        // Arrange
        var cacheKey = "test-key";
        
        _memoryCacheMock
            .Setup(m => m.Remove(cacheKey))
            .Throws(new InvalidOperationException("Cache error"));

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
    
    // Helper callback to mock out parameters
    public delegate void OutCallback<TIn, TOut>(TIn input, out TOut output);
}
