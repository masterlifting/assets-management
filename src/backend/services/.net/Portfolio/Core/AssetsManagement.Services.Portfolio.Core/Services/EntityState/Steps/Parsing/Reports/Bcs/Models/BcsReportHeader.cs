namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.Reports.Bcs.Models
{
    public sealed class BcsReportHeader
    {
        public string Agreement { get; init; } = null!;
        public DateOnly DateStart { get; init; }
        public DateOnly DateEnd { get; init; }
    }
}