namespace AM.Portfolio.Core.Abstractions.Services.DataHeapServices;

public interface IDataHeapParser<out T> where T : class
{
    T Parse(string source, byte[] payload);
}
