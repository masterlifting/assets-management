namespace Shared.Persistense.Abstractions.Entities;

public abstract class SharedEntity : IEntity
{
    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    public string? Info { get; set; }
}