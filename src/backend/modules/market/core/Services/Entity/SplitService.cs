using AM.Services.Market.Domain.Entities;
using AM.Services.Market.Services.Data;

namespace AM.Services.Market.Services.Entity;

public sealed class SplitService
{
    public DataLoader<Split> Loader { get; }

    public SplitService(DataLoader<Split> loader) => Loader = loader;
}