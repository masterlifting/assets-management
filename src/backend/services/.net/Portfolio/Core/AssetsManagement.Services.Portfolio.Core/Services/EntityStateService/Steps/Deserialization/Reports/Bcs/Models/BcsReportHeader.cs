namespace AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Deserialization.Reports.Bcs.Models
{
    public sealed class BcsReportHeader
    {
        public string Agreement { get; init; } = null!;
        public DateOnly DateStart { get; init; }
        public DateOnly DateEnd { get; init; }
    }
}