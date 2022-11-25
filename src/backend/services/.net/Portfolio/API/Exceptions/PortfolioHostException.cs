using Shared.Exceptions.Abstractions;
using Shared.Exceptions.Models;

namespace AM.Services.Portfolio.API.Exceptions;

public sealed class PortfolioHostException : SharedException
{
    public PortfolioHostException(string initiator, string action, ExceptionDescription description) : base(initiator, action, description) { }
}