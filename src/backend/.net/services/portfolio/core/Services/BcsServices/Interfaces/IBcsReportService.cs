using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Core.Services.BcsServices.Models;

namespace AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;

public interface IBcsReportService
{
    BcsReportModel GetReportModel(string fileName, byte[] payload);
    Task<Deal[]> GetDealsAsync(Guid userId, string agreement, IList<BcsReportDealModel> models);
    Task<Event[]> GetEventsAsync(Guid userId, string agreement, IList<BcsReportEventModel> models);
}