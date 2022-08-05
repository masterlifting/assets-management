using System;

namespace AM.Services.Common.Contracts.Models.Service;

public class PaginationModel<T> where T : class
{
    public T[] Items { get; init; } = Array.Empty<T>();
    public int Count { get; init; }
}