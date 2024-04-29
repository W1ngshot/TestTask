using TestTask.ConversionReportApp.Domain.Common;
using TestTask.ConversionReportApp.Domain.Models;
using TestTask.ConversionReportApp.Infrastructure.Entities;

namespace TestTask.ConversionReportApp.Infrastructure.EntityExtensions;

public static class ConversionReportEntityV1Extensions
{
    private static ConversionReportEntityV1 ToReportEntity(this ConversionReport report)
    {
        return new ConversionReportEntityV1
        {
            ItemId = report.ItemId,
            RegistrationId = report.RegistrationId,
            RequestFrom = report.ConversionDateFrom,
            RequestTo = report.ConversionDateTo,
            RequestedAt = report.RequestedAt,
            ConversionRatio = report.ConversionRatio,
            PaymentsCount = report.PaymentsCount
        };
    }

    public static ConversionReportEntityV1[] ToReportEntities(this IEnumerable<ConversionReport> reports) =>
        reports.ToArrayBy(ToReportEntity);

    private static ConversionReport ToDomainReport(this ConversionReportEntityV1 entity)
    {
        return new ConversionReport
        {
            ItemId = entity.ItemId,
            RegistrationId = entity.RegistrationId,
            ConversionDateFrom = entity.RequestFrom,
            ConversionDateTo = entity.RequestTo,
            RequestedAt = entity.RequestedAt,
            ConversionRatio = entity.ConversionRatio,
            PaymentsCount = entity.PaymentsCount
        };
    }

    public static ConversionReport[] ToDomainReports(this IEnumerable<ConversionReportEntityV1> entities) =>
        entities.ToArrayBy(ToDomainReport);
}