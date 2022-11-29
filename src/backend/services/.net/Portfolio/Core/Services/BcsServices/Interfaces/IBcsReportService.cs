using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Core.Services.BcsServices.Models;

namespace AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;

public interface IBcsReportService
{
    BcsReportModel GetReportModel(byte[] payload);
    Deal[] GetDeals(BcsReportModel reportModel);
    Event[] GetEvents(BcsReportModel reportModel);
}