using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using System.ComponentModel.DataAnnotations;
using Shared.Persistense.Abstractions.Entities.State;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

public class Report : IEntityState
{
    [Key, StringLength(40)] 
    public string Id { get; init; } = null!;

    [Required, StringLength(200, MinimumLength = 3)]
    public string Name { get; init; } = null!;

    [Required, StringLength(30, MinimumLength = 2)]
    public string ContentType { get; init; } = null!;
    public byte[] Payload { get; init; } = null!;

    public virtual Provider Provider { get; init; } = null!;
    public int ProviderId { get; init; }

    public virtual User User { get; set; } = null!;
    public string UserId { get; init; } = null!;

    public virtual Account? Account{ get; set; }
    public int? AccountId { get; set; }

    public DateOnly? DateStart { get; set; }
    public DateOnly? DateEnd { get; set; }

    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    public int StateId { get; set; }
    public virtual State State { get; set; } = null!;
    public int StepId { get; set; }
    public virtual Step Step { get; set; } = null!;
    public byte Attempt { get; set; }
    public string? Info { get; set; }
}