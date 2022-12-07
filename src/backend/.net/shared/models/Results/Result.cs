namespace Shared.Models.Results;

public sealed record Result(bool IsSuccess, string? Error = null);