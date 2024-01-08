using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Infrastructure.Settings;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

using Shared.Persistence.Contexts;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Contexts;

public class MongoPortfolioContext : MongoContext
{
    public MongoPortfolioContext(IOptions<DatabaseConnectionSection> options) : base(options.Value.MongoDB)
    {
    }
    public override void OnModelCreating(MongoModelBuilder builder)
    {
        //BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        builder.CreateCollection<IncomingData>().Indexes
        .CreateOne(new CreateIndexModel<IncomingData>(
            Builders<IncomingData>.IndexKeys.Ascending(x => x.PayloadHash)
            , new CreateIndexOptions { Unique = true }));

        //builder.SetCollection<ProcessSteps>(new ProcessSteps[]
        //{
        //new(){Id = (int)Core.Constants.Enums.ProcessSteps.ParseBcsReport, Name = nameof(Core.Constants.Enums.ProcessSteps.ParseBcsReport)}
        //})
        //.Indexes
        //.CreateOne(new CreateIndexModel<ProcessSteps>(
        //    Builders<ProcessSteps>.IndexKeys.Ascending(x => x.Name)
        //    , new CreateIndexOptions { Unique = true }));
    }
}
