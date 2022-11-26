using AM.Services.Portfolio.Infrastructure.Settings;

using Shared.Persistense.Contexts;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Contexts
{
    public class MongoDBPortfolioContext : MongoDBContext
    {
        public MongoDBPortfolioContext(DatabaseConnectionSection connectionSection) : base(connectionSection.MongoDB)
        {
        }
    }
}
