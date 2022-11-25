﻿using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;

public interface IUserRepository : IEntityRepository<User>
{
    ValueTask<User?> FindAsync(Guid userId);
}