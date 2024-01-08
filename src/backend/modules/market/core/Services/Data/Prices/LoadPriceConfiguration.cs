using AM.Services.Market.Clients;
using AM.Services.Market.Domain.DataAccess;
using AM.Services.Market.Domain.DataAccess.Comparators;
using AM.Services.Market.Domain.Entities;
using AM.Services.Market.Services.Data.Prices.Implementations;
using AM.Services.Market.Services.Tasks;
using AM.Services.Market.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using static AM.Services.Market.Enums;

namespace AM.Services.Market.Services.Data.Prices;

public sealed class LoadPriceConfiguration : IDataLoaderConfiguration<Price>
{
    public Func<Price, bool> IsCurrentDataCondition { get; }
    public IEqualityComparer<Price> Comparer { get; }
    public ILastDataHelper<Price> LastDataHelper { get; }
    public DataGrabber<Price> Grabber { get; }

    public LoadPriceConfiguration(ILogger<DataLoader<Price>> logger, IOptions<ServiceSettings> options, IMemoryCache cache, Repository<Price> repository, MoexClient moexClient, TdAmeritradeClient tdAmeritradeClient)
    {
        var settings = options.Value.LoadData.Tasks.FirstOrDefault(x => x.Name.Equals(nameof(LoadPriceTask), StringComparison.OrdinalIgnoreCase)) ?? new();
        Grabber = new(new()
        {
            {(byte) Sources.Moex, new MoexGrabber(logger, cache, moexClient)},
            {(byte) Sources.Tdameritrade, new TdameritradeGrabber(tdAmeritradeClient)}
        });
        IsCurrentDataCondition = x => IsCurrentData(x.SourceId, x.Date);
        Comparer = new DataDateComparer<Price>();
        LastDataHelper = new LastDateHelper<Price>(repository, settings.DaysAgo);
    }

    private static readonly Dictionary<byte, DateOnly[]> exchangeWeekend = new()
    {
        {
            (byte)Sources.Moex,
            new DateOnly[]
            {
                new(2021, 06, 14),
            }
        },
        {
            (byte)Sources.Tdameritrade,
            new DateOnly[]
            {
                new(2021, 05, 31),
            }
        }
    };
    private static bool IsCurrentData(byte sourceId, DateOnly date)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var lastDate = GetLastWorkday(sourceId, date);

        if (lastDate == currentDate)
            return true;
        
        if (lastDate.DayOfWeek is DayOfWeek.Friday)
            lastDate = lastDate.AddDays(3);
        else if (lastDate.AddDays(1) == currentDate)
            lastDate = currentDate;
        return currentDate == lastDate;

        DateOnly GetLastWorkday(byte key, DateOnly checkingDate) =>
            IsExchangeRestday(key, checkingDate)
                ? GetLastWorkday(key, checkingDate.AddDays(-1))
                : checkingDate;
    }
    private static bool IsExchangeRestday(byte key, DateOnly chackingDate) =>
        chackingDate.DayOfWeek is DayOfWeek.Sunday or DayOfWeek.Saturday
        && exchangeWeekend.ContainsKey(key)
        && exchangeWeekend[key].Contains(chackingDate);
}