using AM.Services.Market.Domain.DataAccess;
using AM.Services.Market.Domain.Entities;
using AM.Services.Market.Services.Data;
using AM.Services.Market.Services.Helpers;

namespace AM.Services.Market.Services.Entity;

public sealed class ReportService : StatusChanger<Report>
{
    public DataLoader<Report> Loader { get; }

    public ReportService(Repository<Report> reportRepo, DataLoader<Report> loader) : base(reportRepo) => Loader = loader;
}