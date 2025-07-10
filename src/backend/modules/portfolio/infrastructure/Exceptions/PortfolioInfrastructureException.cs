using Shared.Exceptions.Abstractions;
using Shared.Exceptions.Models;

namespace AM.Services.Portfolio.Infrastructure.Exceptions;

public sealed class PortfolioInfrastructureException : SharedException
{
    public PortfolioInfrastructureException(string initiator, string action, ExceptionDescription description) : base(initiator, action, description) { }
}