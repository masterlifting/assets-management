using Shared.Exceptions.Models;

namespace Shared.Exceptions.Abstractions;

public abstract class SharedException : Exception
{
    public ExceptionModel Model { get; }

    protected SharedException(string initiator, string action, ExceptionDescription description) =>
        Model = new(initiator, action, description.Value);

    public override string Message => $"{Model.Initiator}. {Model.Action}. {Model.Description}";
}