using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Infrastructure.Settings;

using Microsoft.Extensions.Options;

using Shared.Persistence.Contexts;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Contexts;

public class MongoDBPortfolioContext : MongoDBContext
{
    public MongoDBPortfolioContext(IOptions<DatabaseConnectionSection> options) : base(options.Value.MongoDB)
    {
        
    }
    public override void OnModelCreating(MongoDBModelBuilder builder)
    {
        builder.SetCollection<IncomingData>(new());
    }
}
