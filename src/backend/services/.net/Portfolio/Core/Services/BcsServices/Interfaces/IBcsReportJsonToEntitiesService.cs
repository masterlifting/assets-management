using AM.Services.Portfolio.Core.Domain.EntityStateModels.Report.Bcs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

namespace AM.Services.Portfolio.Core.Services.BcsServices.Interfaces
{
    public interface IBcsReportJsonToEntitiesService
    {
        Deal[] GetDeals(BcsReportModel reportModel);
        Event[] GetEvents(BcsReportModel reportModel);
    }
}