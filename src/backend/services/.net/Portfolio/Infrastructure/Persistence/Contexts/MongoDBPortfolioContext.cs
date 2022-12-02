﻿using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Infrastructure.Settings;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

using Shared.Persistence.Contexts;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Contexts;

public class MongoDBPortfolioContext : MongoDBContext
{
    public MongoDBPortfolioContext(IOptions<DatabaseConnectionSection> options) : base(options.Value.MongoDB)
    {
        
    }
    public override void OnModelCreating(MongoDBModelBuilder builder)
    {
        //BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        builder.SetCollection<IncomingData>().Indexes
        .CreateOne(new CreateIndexModel<IncomingData>(
            Builders<IncomingData>.IndexKeys.Ascending(x => x.PayloadHash)
            , new CreateIndexOptions { Unique = true }));
        
        builder.SetCollection<ProcessSteps>( new ProcessSteps[]
        {
            new(){Id = (int)Core.Constants.Enums.ProcessSteps.ParseBcsReport, Name = nameof(Core.Constants.Enums.ProcessSteps.ParseBcsReport)}
        })
        .Indexes
        .CreateOne(new CreateIndexModel<ProcessSteps>(
            Builders<ProcessSteps>.IndexKeys.Ascending(x => x.Name)
            , new CreateIndexOptions { Unique = true }));
    }
}
