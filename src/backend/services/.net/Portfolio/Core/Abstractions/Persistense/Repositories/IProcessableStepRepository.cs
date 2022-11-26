using AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;

using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories
{
    public interface IProcessableStepRepository : ICatalogRepository<ProcessStep>
    {
    }
}
