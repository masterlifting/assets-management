using AM.Services.Portfolio.Core.Domain.EntityStateModels.Report.Bcs;

namespace AM.Services.Portfolio.Core.Services.BcsServices.Interfaces
{
    public interface IBcsReportDataToJsonService
    {
        BcsReportModel GetReportModel(byte[] payload);
    }
}