using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Context;
using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class DataAsJsonRepository<TContext> : SqlProcessableEntityRepository<DataAsJson, TContext>, IDataAsJsonRepository
    where TContext : DbContext, IProcessableDbContext
{
    public DataAsJsonRepository(ILogger<DataAsJson> logger, TContext context) : base(logger, context)
    {
    }
}