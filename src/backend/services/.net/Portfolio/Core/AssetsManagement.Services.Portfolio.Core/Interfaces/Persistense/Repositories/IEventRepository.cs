﻿using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using Shared.Infrastructure.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

public interface IEventRepository : IEntityStateRepository<Event>
{
}