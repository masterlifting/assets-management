using static Shared.Models.Constants.Enums;

namespace Shared.Models.Results;

public sealed record TryResult<T>
{
    public TryResult(T Data)
    {
        this.Data = Data;
        Status = TryResultStatuses.FullSuccess;
        Errors = Array.Empty<string>();
    }
    public TryResult(string[] Errors)
    {
        Status = TryResultStatuses.Unsuccess;
        this.Errors = Errors;
    }
    public TryResult(Exception exception)
    {
        Status = TryResultStatuses.Unsuccess;
        Errors = exception.InnerException is null
            ? new string[] { exception.Message }
            : new string[] { exception.Message, exception.InnerException.Message };
    }
    public TryResult(T Data, string[] Errors)
    {
        Status = TryResultStatuses.PartialSuccess;
        this.Data = Data;
        this.Errors = Errors;
    }

    public TryResultStatuses Status { get; }
    public T? Data { get; }
    public string[] Errors { get; }
}