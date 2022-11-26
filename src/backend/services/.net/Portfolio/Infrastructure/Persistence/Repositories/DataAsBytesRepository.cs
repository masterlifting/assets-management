using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Context;
using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class DataAsBytesRepository<TContext> : SqlProcessableEntityRepository<DataAsBytes, TContext>, IDataAsBytesRepository
    where TContext : DbContext, IProcessableDbContext
{
    public DataAsBytesRepository(ILogger<DataAsBytes> logger, TContext context) : base(logger, context)
    {
    }
}