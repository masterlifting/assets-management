using Shared.Persistense.Entities.EntityData;

using System.Text.Json;

namespace Shared.Persistense.Abstractions.Entities.EntityData
{
    public interface IEntityData : IEntity
    {
        string Name { get; init; }
        string Source { get; init; }

        byte[] Payload { get; init; }
        JsonDocument? Json { get; set; }

        ContentType ContentType { get; init; }
        int ContentTypeId { get; init; }
    }
}