using AM.Services.Portfolio.Core.Domain.Persistense.Collections.BcsReport;

namespace AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;

public interface IBcsReportDataToJsonService
{
    BcsReportModel GetReportModel(byte[] payload);
}