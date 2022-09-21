namespace Shared.Contracts.Models.Results
{
    public sealed record ResultData<T>(Result Result, T? Data) where T : class;
}