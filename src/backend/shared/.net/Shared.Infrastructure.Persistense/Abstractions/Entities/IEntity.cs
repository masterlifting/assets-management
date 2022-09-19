namespace Shared.Infrastructure.Persistense.Abstractions.Entities;

public interface IEntity
{
    public DateTime UpdateTime { get; set; }
}