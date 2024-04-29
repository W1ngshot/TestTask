using AutoBogus;
using Moq;
using TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;
using TestTask.ConversionReportApp.Domain.Models;
using TestTask.ConversionReportApp.Domain.Services;
using TestTask.ConversionReportApp.Domain.Services.Interfaces;
using TestTask.ConversionReportApp.UnitTests.Fakers;

namespace TestTask.ConversionReportApp.UnitTests.DomainServiceTests;

public class ConversionRequestServiceTests
{
    private readonly Mock<IConversionRequestRepository> _requestRepositoryFake = new(MockBehavior.Strict);
    private readonly Mock<IConversionSystem> _conversionSystemFake = new(MockBehavior.Strict);
    private readonly Mock<IConversionReportRepository> _reportRepositoryFake = new(MockBehavior.Strict);
    private readonly Mock<IDateTimeProvider> _dateTimeProviderFake = new(MockBehavior.Strict);

    private readonly IConversionRequestService _requestService;

    public ConversionRequestServiceTests()
    {
        _requestService = new ConversionRequestService(_requestRepositoryFake.Object, _conversionSystemFake.Object,
            _reportRepositoryFake.Object, _dateTimeProviderFake.Object);
    }

    [Fact]
    public async Task AddRequests_ValidInput_ShouldCallRequestRepository()
    {
        const int elementsCount = 2;
        var requests = new AutoFaker<ConversionRequest>().Generate(elementsCount).ToArray();

        _requestRepositoryFake.Setup(repository => repository.AddRequestsAsync(requests, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _requestService.AddRequests(requests, new CancellationToken());

        _requestRepositoryFake.Verify(
            repository => repository.AddRequestsAsync(requests, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessOrdinaryRequests_EmptyRepository_ShouldCallGetOldestRequestsOnce()
    {
        const int requestLimit = 1;

        _dateTimeProviderFake.Setup(provider => provider.OffsetUtcNow)
            .Returns(DateTimeOffset.UtcNow);
        _requestRepositoryFake.Setup(repository =>
                repository.GetOldestRequestsAsync(requestLimit, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ConversionRequest>());

        await _requestService.ProcessOrdinaryRequests(requestLimit, new CancellationToken());

        _requestRepositoryFake.Verify(repository =>
                repository.GetOldestRequestsAsync(requestLimit, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessOrdinaryRequests_RequestTimeOlderThanCurrent_ShouldCallGetOldestRequestsOnce()
    {
        const int requestLimit = 1;
        var expectedTimeOffset = DateTimeOffset.UtcNow;
        var expectedBiggerOffset = expectedTimeOffset + TimeSpan.FromSeconds(1);
        var expectedRequests = new ConversionRequestFaker(expectedBiggerOffset).Generate(1);

        _dateTimeProviderFake.Setup(provider => provider.OffsetUtcNow)
            .Returns(expectedTimeOffset);
        _requestRepositoryFake.Setup(repository =>
                repository.GetOldestRequestsAsync(requestLimit, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRequests);

        await _requestService.ProcessOrdinaryRequests(requestLimit, new CancellationToken());

        _requestRepositoryFake.Verify(repository =>
                repository.GetOldestRequestsAsync(requestLimit, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessOrdinaryRequests_RequestToProcessExists_ShouldCallAllDependencies()
    {
        const int requestLimit = 1;
        var expectedTimeOffset = DateTimeOffset.UtcNow;
        var expectedLowerOffset = expectedTimeOffset - TimeSpan.FromSeconds(1);
        var expectedRequests = new ConversionRequestFaker(expectedLowerOffset).Generate(requestLimit);
        var calculationResponses = new AutoFaker<CalculateConversionResponse>().Generate(requestLimit);

        _dateTimeProviderFake.Setup(provider => provider.OffsetUtcNow)
            .Returns(expectedTimeOffset);
        _requestRepositoryFake.SetupSequence(repository =>
                repository.GetOldestRequestsAsync(requestLimit, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRequests)
            .ReturnsAsync(Array.Empty<ConversionRequest>());
        _requestRepositoryFake.Setup(r => r.RemoveRequestsAsync(expectedRequests, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _conversionSystemFake.Setup(system =>
                system.CalculateConversionsAsync(It.IsAny<IEnumerable<CalculateConversionRequest>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(calculationResponses);
        _reportRepositoryFake.Setup(r =>
                r.AddReportsAsync(It.IsAny<IEnumerable<ConversionReport>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _requestService.ProcessOrdinaryRequests(requestLimit, new CancellationToken());


        _requestRepositoryFake.Verify(repository =>
                repository.GetOldestRequestsAsync(requestLimit, It.IsAny<CancellationToken>()),
            Times.Exactly(2));
        _requestRepositoryFake.Verify(
            repository => repository.RemoveRequestsAsync(expectedRequests, It.IsAny<CancellationToken>()),
            Times.Once);
        _conversionSystemFake.Verify(system =>
                system.CalculateConversionsAsync(It.IsAny<IEnumerable<CalculateConversionRequest>>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
        _reportRepositoryFake.Verify(r =>
                r.AddReportsAsync(It.IsAny<IEnumerable<ConversionReport>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessImmediateRequests_EmptyRepository_ShouldCallGetOldestRequestsOnce()
    {
        const int requestLimit = 1;
        const int timeToImmediate = 1;
        var request = new ProcessImmediateRequest
        {
            BatchSize = requestLimit,
            TimeToBeImmediateInSeconds = timeToImmediate
        };

        _dateTimeProviderFake.Setup(provider => provider.OffsetUtcNow)
            .Returns(DateTimeOffset.UtcNow);
        _requestRepositoryFake.Setup(repository =>
                repository.GetOldestRequestsAsync(requestLimit, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ConversionRequest>());

        await _requestService.ProcessImmediateRequests(request, new CancellationToken());

        _requestRepositoryFake.Verify(repository =>
                repository.GetOldestRequestsAsync(requestLimit, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessImmediateRequests_FirstRequestNotImmediate_ShouldCallGetOldestRequestsOnce()
    {
        const int requestLimit = 1;
        const int timeToImmediate = 1;
        var request = new ProcessImmediateRequest
        {
            BatchSize = requestLimit,
            TimeToBeImmediateInSeconds = timeToImmediate
        };
        var expectedTimeOffset = DateTimeOffset.UtcNow;
        var expectedNotImmediate =
            expectedTimeOffset - TimeSpan.FromDays(1) + TimeSpan.FromSeconds(timeToImmediate) + TimeSpan.FromSeconds(1);
        var expectedRequests = new ConversionRequestFaker(expectedNotImmediate).Generate(1);

        _dateTimeProviderFake.Setup(provider => provider.OffsetUtcNow)
            .Returns(expectedTimeOffset);
        _requestRepositoryFake.Setup(repository =>
                repository.GetOldestRequestsAsync(requestLimit, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRequests);

        await _requestService.ProcessImmediateRequests(request, new CancellationToken());

        _requestRepositoryFake.Verify(repository =>
                repository.GetOldestRequestsAsync(requestLimit, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessImmediateRequests_RequestToProcessExists_ShouldCallAllDependencies()
    {
        const int requestLimit = 1;
        const int timeToImmediate = 1;
        var request = new ProcessImmediateRequest
        {
            BatchSize = requestLimit,
            TimeToBeImmediateInSeconds = timeToImmediate
        };
        var expectedTimeOffset = DateTimeOffset.UtcNow;
        var expectedLowerOffset =
            expectedTimeOffset - TimeSpan.FromDays(1) + TimeSpan.FromSeconds(timeToImmediate) - TimeSpan.FromSeconds(1);
        var expectedRequests = new ConversionRequestFaker(expectedLowerOffset).Generate(requestLimit);
        var calculationResponses = new AutoFaker<CalculateConversionResponse>().Generate(requestLimit);

        _dateTimeProviderFake.Setup(provider => provider.OffsetUtcNow)
            .Returns(expectedTimeOffset);
        _requestRepositoryFake.SetupSequence(repository =>
                repository.GetOldestRequestsAsync(requestLimit, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRequests)
            .ReturnsAsync(Array.Empty<ConversionRequest>());
        _requestRepositoryFake.Setup(r => r.RemoveRequestsAsync(expectedRequests, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _conversionSystemFake.Setup(system =>
                system.CalculateConversionsAsync(It.IsAny<IEnumerable<CalculateConversionRequest>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(calculationResponses);
        _reportRepositoryFake.Setup(r =>
                r.AddReportsAsync(It.IsAny<IEnumerable<ConversionReport>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _requestService.ProcessImmediateRequests(request, new CancellationToken());


        _requestRepositoryFake.Verify(repository =>
                repository.GetOldestRequestsAsync(requestLimit, It.IsAny<CancellationToken>()),
            Times.Exactly(2));
        _requestRepositoryFake.Verify(
            repository => repository.RemoveRequestsAsync(expectedRequests, It.IsAny<CancellationToken>()),
            Times.Once);
        _conversionSystemFake.Verify(system =>
                system.CalculateConversionsAsync(It.IsAny<IEnumerable<CalculateConversionRequest>>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
        _reportRepositoryFake.Verify(r =>
                r.AddReportsAsync(It.IsAny<IEnumerable<ConversionReport>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}