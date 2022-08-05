using AM.Services.Common.Contracts.Models.Entity.Interfaces;
using AM.Services.Market.Domain.Entities.Interfaces;

namespace AM.Services.Market.Services.Data;

public interface IDataLoaderConfiguration<TEntity> where TEntity : class, IDataIdentity, IPeriod
{
    public Func<TEntity, bool> IsCurrentDataCondition { get; }
    public IEqualityComparer<TEntity> Comparer { get; }
    public ILastDataHelper<TEntity> LastDataHelper { get; }
    public DataGrabber<TEntity> Grabber { get; }
}