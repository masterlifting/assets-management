using IM.Service.MarketData.Clients;
using IM.Service.MarketData.Domain.DataAccess;
using IM.Service.MarketData.Domain.Entities;
using IM.Service.MarketData.Domain.Entities.ManyToMany;
using IM.Service.MarketData.Services.DataLoaders.Floats.Implementations;

namespace IM.Service.MarketData.Services.DataLoaders.Floats;

public class FloatLoader : DataLoader<Float>
{
    public FloatLoader(
        ILogger<DataLoader<Float>> logger,
        Repository<Float> repository,
        Repository<CompanySource> companySourceRepo,
        InvestingClient investingClient)
        : base(logger, repository, companySourceRepo, new Dictionary<byte, IDataGrabber>
        {
            { (byte)Enums.Sources.Investing, new InvestingGrabber(repository, logger, investingClient) }
        })
    {
        IsCurrentDataCondition = x => x.Date == DateOnly.FromDateTime(DateTime.UtcNow);
        TimeAgo = 1;
    }
}