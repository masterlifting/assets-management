using System;

namespace CommonServices.Models.Http
{
    public class ResponseModel<T> where T : class
    {
        public T? Data { get; init; }
        public string[] Errors { get; init; } = Array.Empty<string>();
    }
}