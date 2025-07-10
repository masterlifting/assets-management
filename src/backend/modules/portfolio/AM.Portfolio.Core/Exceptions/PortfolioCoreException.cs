using Net.Shared.Exceptions;

namespace AM.Portfolio.Core.Exceptions;

public sealed class PortfolioCoreException : NetSharedException
{
    public PortfolioCoreException(string message) : base(message)
    {
    }

    public PortfolioCoreException(Exception exception) : base(exception)
    {
    }
}
