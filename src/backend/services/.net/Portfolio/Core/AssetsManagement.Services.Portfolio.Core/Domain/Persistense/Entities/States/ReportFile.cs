using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using System.ComponentModel.DataAnnotations;
using Shared.Persistense.Abstractions.Entities.EntityFile;
using Shared.Persistense.Entities.EntityState;
using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Entities.EntityFile;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

public sealed class ReportFile : IEntityState, IEntityFile
{
    [Key, StringLength(40)]
    public string Id { get; init; } = null!;

    [Required, StringLength(200, MinimumLength = 3)]
    public string Name { get; init; } = null!;

    public string Source { get; init; } = null!;
    public byte[] Payload { get; init; } = null!;
    public ContentType ContentType { get; init; } = null!;
    public int ContentTypeId { get; init; }

    public int StateId { get; set; }
    public State State { get; set; } = null!;
    public int StepId { get; set; }
    public Step Step { get; set; } = null!;
    public byte Attempt { get; set; }
    
    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    public string? Info { get; set; }

    public Provider Provider { get; init; } = null!;
    public int ProviderId { get; init; }

    public User User { get; set; } = null!;
    public string UserId { get; init; } = null!;

   public Report? Report { get; init; }
}