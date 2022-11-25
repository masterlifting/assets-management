namespace AM.Services.Portfolio.Core.Domain.EntityStateModels.Report.Bcs;

public sealed class BcsReportModel
{
    public string Version = "1.0.0";

    public string Agreement { get; init; } = null!;
    public string DateStart { get; set; } = null!;
    public string DateEnd { get; set; } = null!;

    public IEnumerable<BcsEventTypeModel>? Events { get; set; }
    public IEnumerable<BcsDealTypeModel>? Deals { get; set; }
}