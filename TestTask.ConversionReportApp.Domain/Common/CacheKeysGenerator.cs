using TestTask.ConversionReportApp.Domain.Models;

namespace TestTask.ConversionReportApp.Domain.Common;

public static class CacheKeysGenerator
{
    public static string GetReportsKey(GetConversionModel model)
    {
        return string.Join(':',
            "item_id", model.ItemId,
            "reg_id", model.RegistrationId,
            model.PageInfo.PageNumber, model.PageInfo.ElementsPerPage);
    }
}