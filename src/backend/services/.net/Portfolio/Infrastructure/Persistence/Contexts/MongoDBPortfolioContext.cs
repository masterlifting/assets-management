using AM.Services.Portfolio.Infrastructure.Settings;

using Microsoft.Extensions.Options;

using Shared.Persistense.Contexts;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Contexts;

public class MongoDBPortfolioContext : MongoDBContext
{
    public MongoDBPortfolioContext(IOptions<DatabaseConnectionSection> options) : base(options.Value.MongoDB)
    {
    }
}
