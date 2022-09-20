using System;
using Shared.Exceptions.Abstractions;

namespace AM.Services.Portfolio.Host.Exceptions;

public sealed class PortfolioHostException : SharedException
{
    private readonly string _message;
    public PortfolioHostException(string initiator, string action, string message)
        : base(initiator, action) => _message = message;
    public PortfolioHostException(string initiator, string action, Exception exception)
        : base(initiator, action) => _message = exception.InnerException?.Message ?? exception.Message;

    public override string Message => base.Message + _message;
}