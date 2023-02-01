using AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Infrastructure.Persistence.Contexts;

using Microsoft.Extensions.Logging;

using Shared.Persistence.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories
{
    public sealed class IncomingDataRepository : MongoRepository<IncomingData, MongoPortfolioContext>, IIncomingDataRepository
    {
        public IncomingDataRepository(ILogger<IncomingData> logger, MongoPortfolioContext context) : base(logger, context)
        {
        }
    }
}
