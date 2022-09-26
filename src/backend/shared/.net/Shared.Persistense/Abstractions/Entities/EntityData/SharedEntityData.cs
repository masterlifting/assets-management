using Shared.Persistense.Entities.EntityData;

using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Shared.Persistense.Abstractions.Entities.EntityData
{
    public abstract class SharedEntityData : SharedEntity, IEntityData
    {
        [Required, StringLength(200, MinimumLength = 3)]
        public string Name { get; init; } = null!;
        [Required, StringLength(300, MinimumLength = 5)]
        public string Source { get; init; } = null!;

        public byte[] Payload { get; init; } = Array.Empty<byte>();
        public JsonDocument? Json { get; set; }

        public ContentType ContentType { get; init; } = null!;
        public int ContentTypeId { get; init; }
    }
}