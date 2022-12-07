namespace Shared.Persistence.Abstractions.Entities;

public interface IPersistent
{
    DateTime Created { get; init; }
    string? Info { get; set; }
}