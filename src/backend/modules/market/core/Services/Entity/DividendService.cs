using AM.Services.Market.Domain.DataAccess;
using AM.Services.Market.Domain.Entities;
using AM.Services.Market.Services.Data;
using AM.Services.Market.Services.Helpers;

namespace AM.Services.Market.Services.Entity;

public sealed class DividendService : StatusChanger<Dividend>
{
    public DataLoader<Dividend> Loader { get; }

    public DividendService(Repository<Dividend> reportRepo, DataLoader<Dividend> loader) : base(reportRepo)
    {
        Loader = loader;
    }
}