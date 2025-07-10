using AM.Portfolio.Core.Persistence.Entities.NoSql;
using AM.Portfolio.Core.Persistence.Entities.NoSql.Catalogs;
using AM.Portfolio.Infrastructure.Settings;

using Microsoft.Extensions.Options;
using MongoDB.Driver;

using Net.Shared.Persistence.Contexts;

using static AM.Portfolio.Core.Constants;

namespace AM.Portfolio.Infrastructure.Persistence.Contexts;

public class MongoDbPortfolioContext : MongoDbContext
{
    public MongoDbPortfolioContext(IOptions<DatabaseConnectionSection> options) : base(options.Value.MongoDb)
    {
    }
    public override void OnModelCreating(MongoDbBuilder builder)
    {
        builder
            .SetCollection<DataHeap>()
            .Indexes
            .CreateOne(new CreateIndexModel<DataHeap>(
                Builders<DataHeap>.IndexKeys.Ascending(x => x.PayloadHash),
                new CreateIndexOptions { Unique = true }));

        builder
            .SetCollection(new ProcessSteps[]
                {
                    new(){Id = (int)Enums.ProcessSteps.None, Name = nameof(Enums.ProcessSteps.None)},
                    new(){Id = (int)Enums.ProcessSteps.CalculateSplitting, Name = nameof(Enums.ProcessSteps.CalculateSplitting)},
                    new(){Id = (int)Enums.ProcessSteps.CalculateBalance, Name = nameof(Enums.ProcessSteps.CalculateBalance)},
                    new(){Id = (int)Enums.ProcessSteps.ParseBcsCompanies, Name = nameof(Enums.ProcessSteps.ParseBcsCompanies)},
                    new(){Id = (int)Enums.ProcessSteps.ParseBcsTransactions, Name = nameof(Enums.ProcessSteps.ParseBcsTransactions)},
                    new(){Id = (int)Enums.ProcessSteps.ParseRaiffeisenSrbTransactions, Name = nameof(Enums.ProcessSteps.ParseRaiffeisenSrbTransactions)},
                })
            .Indexes
            .CreateOne(new CreateIndexModel<ProcessSteps>(
               Builders<ProcessSteps>.IndexKeys.Ascending(x => x.Name),
               new CreateIndexOptions { Unique = true }));
    }
}
