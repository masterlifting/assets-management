using AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Infrastructure.Persistence.Contexts;

using Microsoft.Extensions.Logging;

using Shared.Persistence.Repositories;

using static AM.Services.Common.Constants.Enums;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories
{
    public sealed class DerivativeRepository : PostgreRepository<Derivative, PostgrePortfolioContext>, IDerivativeRepository
    {
        public DerivativeRepository(ILogger<Derivative> logger, PostgrePortfolioContext context) : base(logger, context)
        {
        }

        public Task<Derivative[]> GetDerivativesAsync(AssetTypes type) => throw new NotImplementedException();
        public Task<Derivative[]> GetDerivativesAsync() => throw new NotImplementedException();
    }
}
