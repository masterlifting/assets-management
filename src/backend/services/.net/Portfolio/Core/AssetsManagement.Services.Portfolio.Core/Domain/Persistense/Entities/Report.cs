using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Shared.Persistense.Abstractions.Entities;

using System.ComponentModel.DataAnnotations;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities;

public sealed class Report : IEntity
{
    [Key]
    public int Id { get; init; }

    public ReportFile ReportFile { get; init; } = null!;
    public string ReportFileId { get; init; } = null!;

    public Account Account { get; set; } = null!;
    public int AccountId { get; init; }

    public DateOnly DateStart { get; init; }
    public DateOnly DateEnd { get; init; }

    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    public string? Info { get; set; }
}