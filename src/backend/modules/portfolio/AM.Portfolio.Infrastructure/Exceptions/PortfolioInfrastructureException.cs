using Net.Shared.Exceptions;

namespace AM.Portfolio.Infrastructure.Exceptions;

public sealed class PortfolioInfrastructureException : NetSharedException
{
    public PortfolioInfrastructureException(string message) : base(message)
    {
    }

    public PortfolioInfrastructureException(Exception exception) : base(exception)
    {
    }
}
