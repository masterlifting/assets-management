using Shared.Exceptions.Abstractions;

namespace AM.Services.Portfolio.Infrastructure.Exceptions
{
    public sealed class PortfolioInfrastructureException : SharedException
    {
        private readonly string _message;
        public PortfolioInfrastructureException(string initiator, string action, string message)
            : base(initiator, action) => _message = message;
        public PortfolioInfrastructureException(string initiator, string action, Exception exception)
            : base(initiator, action) => _message = exception.InnerException?.Message ?? exception.Message;

        public override string Message => base.Message + _message;
    }
}