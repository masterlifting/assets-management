using System.Text.Json;

namespace Shared.Persistense.Abstractions.Entities;

public interface IEntityDataAsJson : IEntityData
{
    public JsonDocument Json { get; init; }
    public string JsonVersion { get; init; }
}
