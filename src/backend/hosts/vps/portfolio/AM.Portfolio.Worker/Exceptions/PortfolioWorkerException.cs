using Net.Shared.Exceptions;

namespace AM.Portfolio.Worker.Exceptions;

public sealed class PortfolioWorkerException : NetSharedException
{
    public PortfolioWorkerException(string message) : base(message)
    {
    }

    public PortfolioWorkerException(Exception exception) : base(exception)
    {
    }
}
