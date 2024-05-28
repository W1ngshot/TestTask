using TestTask.ConversionReportApp.Domain.Common;

namespace TestTask.ConversionReportApp.UnitTests.DomainCommonTests;

public class AsyncEnumerableExtensions
{
    [Fact]
    public async Task Buffer_SourceIsNull_ShouldThrowArgumentNullException()
    {
        IAsyncEnumerable<int>? source = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await source.Buffer(2).ToListAsync());
    }

    [Fact]
    public async Task Buffer_CountIsLessOrEqualToOne_ShouldThrowArgumentOutOfRangeException()
    {
        const int count = 1;
        var source = AsyncEnumerable.Range(1, 5);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await source.Buffer(count).ToListAsync());
    }

    [Fact]
    public async Task Buffer_ElementsMoreThanBufferCount_ShouldReturnCorrectlyBufferedData()
    {
        const int count = 3;
        var source = AsyncEnumerable.Range(1, 5);
        var firstExpectedBuffer = new[] {1, 2, 3};
        var secondExpectedBuffer = new[] {4, 5};

        var result = await source.Buffer(count).ToListAsync();

        Assert.Collection(result,
            items => Assert.Equal(firstExpectedBuffer, items),
            items => Assert.Equal(secondExpectedBuffer, items));
    }
    
    [Fact]
    public async Task Buffer_BufferHasDelayLessThanItems_ShouldReturnCorrectlyBufferedData()
    {
        const int count = 3;
        var delay = TimeSpan.FromMilliseconds(1);
        var source = AsyncEnumerable.Range(1, 2).SelectAwait(async x =>
        {
            await Task.Delay(100);
            return x;
        });
        var firstExpectedBuffer = new[] {1};
        var secondExpectedBuffer = new[] {2};

        var result = await source.Buffer(count, delay).ToListAsync();

        Assert.Collection(result,
            items => Assert.Equal(firstExpectedBuffer, items),
            items => Assert.Equal(secondExpectedBuffer, items));
    }
    
    [Fact]
    public async Task Buffer_BufferHasDelayMoreThanItems_ShouldReturnCorrectlyBufferedData()
    {
        const int count = 3;
        var delay = TimeSpan.FromMilliseconds(100);
        var source = AsyncEnumerable.Range(1, 4).SelectAwait(async x =>
        {
            await Task.Delay(10);
            return x;
        });
        var firstExpectedBuffer = new[] {1, 2, 3};
        var secondExpectedBuffer = new[] {4};

        var result = await source.Buffer(count, delay).ToListAsync();

        Assert.Collection(result,
            items => Assert.Equal(firstExpectedBuffer, items),
            items => Assert.Equal(secondExpectedBuffer, items));
    }
}