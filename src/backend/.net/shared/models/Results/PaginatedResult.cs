namespace Shared.Models.Results;

public record PaginatedResult<T> where T : class
{
    public T[] Items { get; init; } = Array.Empty<T>();
    public int Count { get; init; }
    public int Page { get; set; }
    public int Limit { get; set; }
}