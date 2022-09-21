using Shared.Exceptions.Abstractions;

namespace AM.Services.Portfolio.Core.Exceptions
{
    public sealed class PortfolioCoreException : SharedException
    {
        private readonly string _message;
        public PortfolioCoreException(string initiator, string action, string message)
            : base(initiator, action) => _message = message;
        public PortfolioCoreException(string initiator, string action, Exception exception)
            : base(initiator, action) => _message = exception.InnerException?.Message ?? exception.Message;

        public override string Message => base.Message + _message;
    }
}