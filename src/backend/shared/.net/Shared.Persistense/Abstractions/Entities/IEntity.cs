namespace Shared.Persistense.Abstractions.Entities;

public interface IEntity
{
    DateTime UpdateTime { get; set; }
    string? Info { get; set; }
}