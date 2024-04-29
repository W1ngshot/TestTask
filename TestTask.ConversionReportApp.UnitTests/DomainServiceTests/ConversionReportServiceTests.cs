using AutoBogus;
using Moq;
using TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;
using TestTask.ConversionReportApp.Domain.Models;
using TestTask.ConversionReportApp.Domain.Services;
using TestTask.ConversionReportApp.Domain.Services.Interfaces;
using TestTask.ConversionReportApp.UnitTests.Fakers;

namespace TestTask.ConversionReportApp.UnitTests.DomainServiceTests;

public class ConversionReportServiceTests
{
    private readonly Mock<IConversionReportRepository> _reportRepositoryFake = new(MockBehavior.Strict);
    private readonly Mock<IReportCacheService> _cacheService = new(MockBehavior.Strict);

    private readonly IConversionReportService _reportService;

    public ConversionReportServiceTests()
    {
        _reportService = new ConversionReportService(_reportRepositoryFake.Object, _cacheService.Object);
    }

    [Fact]
    public async Task GetReportsAsync_CacheNotExists_ShouldReturnItemsFromRepository()
    {
        const int reportsCount = 2;
        var request = new GetConversionModel
        {
            ItemId = 1,
            RegistrationId = 1,
            PageInfo = new PageInfoFaker(reportsCount).Generate()
        };
        var expectedReports = new AutoFaker<ConversionReport>().Generate(reportsCount);

        _cacheService.Setup(cache => cache.GetAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<ConversionReport>?) null);
        _cacheService.Setup(cache => cache.SetAsync(It.IsAny<GetConversionModel>(),
                It.IsAny<IEnumerable<ConversionReport>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _reportRepositoryFake.Setup(repository => repository.GetReportsAsync(
                It.Is<GetConversionRequest>(x =>
                    x.ItemId == request.ItemId && x.RegistrationId == request.RegistrationId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedReports);

        var actualReports = await _reportService.GetReportsAsync(request, new CancellationToken());

        Assert.Equal(expectedReports, actualReports);
    }

    [Fact]
    public async Task GetReportsAsync_CacheNotExists_ShouldSetNewCache()
    {
        const int reportsCount = 2;
        var request = new GetConversionModel
        {
            ItemId = 1,
            RegistrationId = 1,
            PageInfo = new PageInfoFaker(reportsCount).Generate()
        };
        var expectedReports = new AutoFaker<ConversionReport>().Generate(reportsCount);

        _cacheService.Setup(cache => cache.GetAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<ConversionReport>?) null);
        _cacheService.Setup(cache => cache.SetAsync(request, expectedReports, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _reportRepositoryFake.Setup(repository => repository.GetReportsAsync(
                It.Is<GetConversionRequest>(x =>
                    x.ItemId == request.ItemId && x.RegistrationId == request.RegistrationId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedReports);

        await _reportService.GetReportsAsync(request, new CancellationToken());

        _cacheService.Verify(cache => cache.SetAsync(request, expectedReports, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetReportsAsync_CacheExists_ShouldReturnItemsFromCache()
    {
        const int reportsCount = 2;
        var request = new GetConversionModel
        {
            ItemId = 1,
            RegistrationId = 1,
            PageInfo = new PageInfoFaker(reportsCount).Generate()
        };
        var expectedReports = new AutoFaker<ConversionReport>().Generate(reportsCount);

        _cacheService.Setup(cache => cache.GetAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedReports);
        _cacheService.Setup(cache => cache.SetAsync(It.IsAny<GetConversionModel>(),
                It.IsAny<IEnumerable<ConversionReport>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _reportRepositoryFake.Setup(repository => repository.GetReportsAsync(
                It.Is<GetConversionRequest>(x =>
                    x.ItemId == request.ItemId && x.RegistrationId == request.RegistrationId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ConversionReport>());

        var actualReports = await _reportService.GetReportsAsync(request, new CancellationToken());

        Assert.Equal(expectedReports, actualReports);
    }

    [Fact]
    public async Task GetReportsAsync_ValidInput_ShouldCountTakeAndSkipCorrectly()
    {
        const int elementsPerPage = 2;
        const int pageNumber = 2;
        var request = new GetConversionModel
        {
            ItemId = 1,
            RegistrationId = 1,
            PageInfo = new PageInfo
            {
                ElementsPerPage = elementsPerPage,
                PageNumber = pageNumber
            }
        };
        const int expectedTake = elementsPerPage;
        const int expectedSkip = (pageNumber - 1) * elementsPerPage;

        _cacheService.Setup(cache => cache.GetAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<ConversionReport>?) null);
        _cacheService.Setup(cache => cache.SetAsync(It.IsAny<GetConversionModel>(),
                It.IsAny<IEnumerable<ConversionReport>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _reportRepositoryFake.Setup(repository => repository.GetReportsAsync(
                It.IsAny<GetConversionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ConversionReport>());

        await _reportService.GetReportsAsync(request, new CancellationToken());

        _reportRepositoryFake.Verify(repository => repository.GetReportsAsync(
            It.Is<GetConversionRequest>(r => r.Take == expectedTake && r.Skip == expectedSkip),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}