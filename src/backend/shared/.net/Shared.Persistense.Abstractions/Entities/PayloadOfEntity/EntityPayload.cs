using Shared.Persistense.Entities.Catalogs;

using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Shared.Persistense.Abstractions.Entities.PayloadOfEntity;

public abstract class EntityPayload : IEntityPayload
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