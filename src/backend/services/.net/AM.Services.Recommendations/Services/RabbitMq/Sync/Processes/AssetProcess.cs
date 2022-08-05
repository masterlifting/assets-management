using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.Models.RabbitMq.Api;
using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Recommendations.Domain.DataAccess;
using AM.Services.Recommendations.Domain.DataAccess.Comparators;
using AM.Services.Recommendations.Domain.Entities;

namespace AM.Services.Recommendations.Services.RabbitMq.Sync.Processes;

public class AssetProcess : IRabbitProcess
{
    private const string serviceName = "asset synchronization";
    private readonly Repository<Asset> assetRepo;
    public AssetProcess(Repository<Asset> assetRepo) => this.assetRepo = assetRepo;

    public Task ProcessAsync<T>(QueueActions action, T model) where T : class => model switch
    {
        AssetMqDto dto => action switch
        {
            QueueActions.Create => assetRepo.CreateAsync(GetAsset(dto), serviceName),
            QueueActions.Update => assetRepo.UpdateAsync(new[] { dto.Id }, GetAsset(dto), serviceName),
            QueueActions.Delete => assetRepo.DeleteAsync(new[] { dto.Id }, serviceName),
            _ => Task.CompletedTask
        },
        _ => Task.CompletedTask
    };
    public Task ProcessRangeAsync<T>(QueueActions action, IEnumerable<T> models) where T : class => models switch
    {
        AssetMqDto[] dtos => action switch
        {
            QueueActions.Set => assetRepo.CreateUpdateDeleteAsync(GetAssets(dtos), new AssetComparer(), serviceName),
            QueueActions.Create => assetRepo.CreateRangeAsync(GetAssets(dtos), new AssetComparer(), serviceName),
            QueueActions.Update => assetRepo.UpdateRangeAsync(GetAssets(dtos), serviceName),
            QueueActions.Delete => assetRepo.DeleteRangeAsync(GetAssets(dtos), serviceName),
            _ => Task.CompletedTask
        },
        _ => Task.CompletedTask
    };

    private static Asset GetAsset(AssetMqDto model) => new()
    {
        Id = model.Id,
        TypeId = model.TypeId,
        CountryId = model.CountryId,
        Name = model.Name
    };
    private static Asset[] GetAssets(IEnumerable<AssetMqDto> models) => models.Select(x => new Asset
    {
        Id = x.Id,
        TypeId = x.TypeId,
        CountryId = x.CountryId,
        Name = x.Name
    })
    .ToArray();
}