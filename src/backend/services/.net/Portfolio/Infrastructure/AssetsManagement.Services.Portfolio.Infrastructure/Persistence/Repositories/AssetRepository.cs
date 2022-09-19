using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Domain.Persistense.Models;
using AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Infrastructure.Persistense.Abstractions.Entities.State.Handle;
using Shared.Infrastructure.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public class AssetRepository<TContext> : EntityStateRepository<Asset, TContext>, IAssetRepository
    where TContext : DbContext, IEntityStateDbContext
{
    protected AssetRepository(ILogger<Asset> logger, TContext context) : base(logger, context)
    {
    }

    public async Task<Asset[]> GetNewAssetsAsync(IEnumerable<AssetModel> models)
    {
        var assetIds = models.Select(x => x.AssetId.AsString);

        var dbAssets = await DbSet
            .Where(x => assetIds.Contains(x.Id))
            .ToArrayAsync();

        var dbAssetIds = dbAssets.Select(x => x.Id).ToArray();

        var newIds = assetIds.Except(dbAssetIds);

        return dbAssets.Where(x => newIds.Contains(x.Id)).ToArray();
    }
}