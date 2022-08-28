namespace Shared.Core.Exceptions;

public abstract class SharedCoreException : Exception
{
    private readonly string _initiator;
    private readonly string _action;

    protected SharedCoreException(string initiator, string action)
    {
        _initiator = initiator;
        _action = action;
    }

    public override string Message => $"{_initiator}.{_action}. Error: ";
}