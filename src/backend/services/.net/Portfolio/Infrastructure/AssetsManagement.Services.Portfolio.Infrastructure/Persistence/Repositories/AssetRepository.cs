using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Domain.Persistense.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense;
using Shared.Persistense.Abstractions.Context;
using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories
{
    public sealed class AssetRepository<TContext> : EntityStateRepository<Asset, TContext>, IAssetRepository
        where TContext : DbContext, IEntityStateDbContext
    {
        public AssetRepository(ILogger<Asset> logger, TContext context) : base(logger, context)
        {
        }

        public override Task CreateAsync(Asset entity, CancellationToken? ctToken = null)
        {
            entity.StepId = (int)Constants.Enums.Steps.Loading;
            return base.CreateAsync(entity, ctToken);
        }
        public override Task CreateRangeAsync(IReadOnlyCollection<Asset> entities, CancellationToken? cToken = null)
        {
            foreach (var entity in entities)
                entity.StepId = (int)Constants.Enums.Steps.Loading;

            return base.CreateRangeAsync(entities, cToken);
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
}