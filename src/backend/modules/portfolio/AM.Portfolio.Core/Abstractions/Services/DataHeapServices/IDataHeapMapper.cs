namespace AM.Portfolio.Core.Abstractions.Services.DataHeapServices;

public interface IDataHeapMapper<in T> where T : class
{
    Task Map(T data, CancellationToken cToken);
}
