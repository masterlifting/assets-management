using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Computing.Deals;

using Microsoft.Extensions.Logging;
using Shared.Background.Abstractions.EntityState;
using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Services.EntityState.Handlers;

public sealed class DealStateHandler : EntityStateHandler<Deal>
{
    public DealStateHandler(
        ILogger<DealStateHandler> logger
        , IDealRepository dealRepository) : base(dealRepository, new()
        {
            {(int)Enums.Steps.Computing, new DealCalculator()}
        })
    {
    }
}