using Shared.Exceptions.Abstractions;

namespace Shared.Exceptions.Models;

public sealed class ExceptionDescription
{
    public string Value { get; }
    public ExceptionDescription(string value)
    {
        Value = value;
    }
    public ExceptionDescription(Exception exception)
    {
        Value = exception.InnerException?.Message ?? exception.Message;
    }
    public ExceptionDescription(SharedException exeption)
    {
        Value = $". {exeption.Model.Initiator}. {exeption.Model.Action}. {exeption.Model.Description}";
    }
}