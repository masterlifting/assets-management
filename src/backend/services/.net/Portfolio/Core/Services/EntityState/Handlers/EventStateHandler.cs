using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Computing.Events;

using Microsoft.Extensions.Logging;
using Shared.Background.Abstractions.EntityState;
using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Services.EntityState.Handlers;

public sealed class EventStateHandler : EntityStateHandler<Event>
{
    public EventStateHandler(
        ILogger<EventStateHandler> logger
        , IEventRepository eventRepository) : base(eventRepository, new()
        {
            {(int)Enums.Steps.Computing, new EventCalculator()}
        })
    {
    }
}