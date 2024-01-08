namespace Shared.Models.Results;

public sealed record TryResult<T>
{
    public TryResult(T Data)
    {
        this.Data = Data;
        IsSuccess = true;
        Errors = Array.Empty<string>();
    }
    public TryResult(string[] Errors)
    {
        IsSuccess = false;
        this.Errors = Errors;
    }
    public TryResult(Exception exception)
    {
        IsSuccess = false;
        Errors = exception.InnerException is null
            ? new string[] { exception.Message }
            : new string[] { exception.Message, exception.InnerException.Message };
    }

    public bool IsSuccess { get; }
    public T? Data { get; }
    public string[] Errors { get; }
}