using AutoBogus;
using TestTask.ConversionReportApp.Domain.Models;

namespace TestTask.ConversionReportApp.UnitTests.Fakers;

public sealed class PageInfoFaker : AutoFaker<PageInfo>
{
    public PageInfoFaker(int elementsCount)
    {
        RuleFor(info => info.ElementsPerPage, _ => elementsCount);
        RuleFor(info => info.PageNumber, _ => 1);
    }
}