using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Report.Api;
using TestTask.ConversionReportApp.Domain.Common;
using TestTask.ConversionReportApp.Domain.Models;
using TestTask.ConversionReportApp.Domain.Services.Interfaces;

namespace TestTask.ConversionReportApp.Presentation.Services;

public class ReportApiService(IConversionReportService reportService) : ReportsApi.ReportsApiBase
{
    public override async Task<GetReportsResponse> GetReports(GetReportsRequest request, ServerCallContext context)
    {
        var model = new GetConversionModel
        {
            ItemId = request.ItemId,
            RegistrationId = request.RegistrationId,
            PageInfo = new PageInfo
            {
                ElementsPerPage = request.Page.ElementsPerPage,
                PageNumber = request.Page.PageNumber
            }
        };
        var reports = (await reportService.GetReportsAsync(model, context.CancellationToken)).ToList();

        var responseReports = reports.ToArrayBy(report => new Report.Api.Report
        {
            ConversionFrom = report.ConversionDateFrom.ToTimestamp(),
            ConversionTo = report.ConversionDateTo.ToTimestamp(),
            PaymentsCount = report.PaymentsCount,
            Ratio = report.ConversionRatio,
            RequestedAt = report.RequestedAt.ToTimestamp()
        });

        var response = new GetReportsResponse
        {
            ItemId = request.ItemId,
            RegistrationId = request.RegistrationId
        };
        response.Reports.AddRange(responseReports);
        return response;
    }
}