using AM.Services.Market.Domain.Entities;
using AM.Services.Market.Services.Data;

namespace AM.Services.Market.Services.Entity;

public sealed class FloatService
{
    public DataLoader<Float> Loader { get; }

    public FloatService(DataLoader<Float> loader)
    {
        Loader = loader;
    }
}