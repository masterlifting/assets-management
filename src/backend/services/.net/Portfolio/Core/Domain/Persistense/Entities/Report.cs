using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

using Shared.Persistense.Abstractions.Entities;

using System.ComponentModel.DataAnnotations;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities;

public sealed class Report : SharedEntity
{
    [Key]
    public int Id { get; init; }

    public ReportData ReportData { get; init; } = null!;
    public string ReportDataId { get; init; } = null!;

    public Account Account { get; set; } = null!;
    public int AccountId { get; init; }

    public DateOnly DateStart { get; init; }
    public DateOnly DateEnd { get; init; }
}