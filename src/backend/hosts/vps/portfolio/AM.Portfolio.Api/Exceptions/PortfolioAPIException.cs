using System;

using Net.Shared.Exceptions;

namespace AM.Portfolio.Api.Exceptions;

public sealed class PortfolioApiException : NetSharedException
{
    public PortfolioApiException(string message) : base(message)
    {
    }

    public PortfolioApiException(Exception exception) : base(exception)
    {
    }
}
