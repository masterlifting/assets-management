using Shared.Exceptions.Abstractions;
using Shared.Exceptions.Models;

namespace AM.Services.Portfolio.Core.Exceptions;

public sealed class PortfolioCoreException : SharedException
{
    public PortfolioCoreException(string initiator, string action, ExceptionDescription description) : base(initiator, action, description) { }
}