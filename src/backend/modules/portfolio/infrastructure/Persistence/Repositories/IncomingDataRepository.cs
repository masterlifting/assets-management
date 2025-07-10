using AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistence.Collections;

using Microsoft.Extensions.Logging;
using Shared.Persistence.Abstractions.Contexts;
using Shared.Persistence.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories
{
    public sealed class IncomingDataRepository : MongoRepository<IncomingData>, IIncomingDataRepository
    {
        public IncomingDataRepository(ILogger<IncomingData> logger, IMongoPersistenceContext context) : base(logger, context)
        {
        }
    }
}
