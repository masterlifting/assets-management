using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Computing.Derivatives;

using Microsoft.Extensions.Logging;
using Shared.Background.Abstractions.EntityState;
using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Services.EntityState.Handlers;

public sealed class DerivativeStateHandler : EntityStateHandler<Derivative>
{
    public DerivativeStateHandler(
        ILogger<DerivativeStateHandler> logger
        , IDerivativeRepository derivativeRepository) : base(derivativeRepository, new()
        {
            {(int)Enums.Steps.Computing, new DerivativeCalculator()}
        })
    {
    }
}