using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using Shared.Infrastructure.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

public interface IUserRepository : IRepository<User>
{
}