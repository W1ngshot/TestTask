using AutoBogus;
using TestTask.ConversionReportApp.Domain.Models;

namespace TestTask.ConversionReportApp.UnitTests.Fakers;

public sealed class ConversionRequestFaker : AutoFaker<ConversionRequest>
{
    public ConversionRequestFaker(DateTimeOffset requestedAt)
    {
        RuleFor(x => x.RequestedAt, _ => requestedAt);
    }
}