namespace Shared.Exceptions.Abstractions;

public abstract class SharedException : Exception
{
    private readonly string _initiator;
    private readonly string _action;

    protected SharedException(string initiator, string action)
    {
        _initiator = initiator;
        _action = action;
    }

    public override string Message => $"Инициатор: {_initiator}; Действие: {_action}; Описание: ";
}