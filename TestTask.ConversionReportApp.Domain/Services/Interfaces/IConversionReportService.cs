using TestTask.ConversionReportApp.Domain.Models;

namespace TestTask.ConversionReportApp.Domain.Services.Interfaces;

public interface IConversionReportService
{
    public Task<IEnumerable<ConversionReport>> GetReportsAsync(GetConversionModel model, CancellationToken token);
}