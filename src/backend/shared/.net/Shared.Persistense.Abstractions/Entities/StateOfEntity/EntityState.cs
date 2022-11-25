using Shared.Persistense.Entities.Catalogs;

using System.ComponentModel.DataAnnotations;

namespace Shared.Persistense.Abstractions.Entities.StateOfEntity;

public abstract class EntityState : IEntityProcessable
{
    [Key, StringLength(50)]
    public string Id { get; init; } = null!;

    public Status Status { get; set; } = null!;
    public int StatusId { get; set; }

    public Step Step { get; init; } = null!;
    public int StepId { get; set; }

    public byte Attempt { get; set; }
}