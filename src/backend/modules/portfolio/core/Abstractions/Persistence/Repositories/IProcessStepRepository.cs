using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;
using Shared.Persistence.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories
{
    public interface IProcessStepRepository : IPersistenceSqlRepository<ProcessStep>
    {
    }
}
