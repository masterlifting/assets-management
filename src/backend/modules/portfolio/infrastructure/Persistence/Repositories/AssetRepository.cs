using AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;

using Microsoft.Extensions.Logging;
using Shared.Persistence.Abstractions.Contexts;
using Shared.Persistence.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class AssetRepository : PostgreRepository<Asset>, IAssetRepository
{
    public AssetRepository(ILogger<Asset> logger, IPostgrePersistenceContext context) : base(logger, context)
    {
    }
}
