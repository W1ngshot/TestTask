using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;
using TestTask.ConversionReportApp.Infrastructure.CacheServices;

namespace TestTask.ConversionReportApp.UnitTests.InfrastructureCacheServicesTests;

public class CacheServiceTests
{
    private readonly Mock<IDistributedCacheWrapper> _distributedCacheFake = new(MockBehavior.Strict);

    private readonly ICacheService<string> _cacheService;

    public CacheServiceTests()
    {
        _cacheService = new CacheService<string>(_distributedCacheFake.Object);
    }

    [Fact]
    public async Task GetAsync_CacheNotExists_ShouldReturnNull()
    {
        const string key = "test";
        _distributedCacheFake.Setup(cache => cache.GetStringAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string) null);

        var result = await _cacheService.GetAsync(key, default);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_CacheExists_ShouldReturnExpectedData()
    {
        const string key = "test";
        const string expectedData = "expected string data";
        var serializedData = JsonSerializer.Serialize(expectedData);
        _distributedCacheFake.Setup(cache => cache.GetStringAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serializedData);

        var result = await _cacheService.GetAsync(key, default);

        Assert.NotNull(result);
        Assert.Equal(expectedData, result);
    }

    [Fact]
    public async Task SetAsync_ValidData_ShouldCallSetStringAsync()
    {
        const string key = "test";
        var expectedData = "test string data";
        var serializedData = JsonSerializer.Serialize(expectedData);
        _distributedCacheFake.Setup(cache => cache.SetStringAsync(key, serializedData,
                It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _cacheService.SetAsync(key, expectedData, default);

        _distributedCacheFake.Verify(cache => cache.SetStringAsync(key, serializedData,
                It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}