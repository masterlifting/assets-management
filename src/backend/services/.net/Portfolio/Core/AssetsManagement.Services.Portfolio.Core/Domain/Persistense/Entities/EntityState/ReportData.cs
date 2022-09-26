using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

using Shared.Persistense.Abstractions.Entities.EntityData;
using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Entities.EntityData;

using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState
{
    public sealed class ReportData : SharedEntityState, IEntityData
    {
        [Required, StringLength(200, MinimumLength = 3)]
        public string Name { get; init; } = null!;
        public string Source { get; init; } = null!;

        public byte[] Payload { get; init; } = null!;
        public JsonDocument? Json { get; set; }

        public ContentType ContentType { get; init; } = null!;
        public int ContentTypeId { get; init; }

        public Provider Provider { get; init; } = null!;
        public int ProviderId { get; init; }

        public User User { get; set; } = null!;
        public string UserId { get; init; } = null!;

        public Report? Report { get; init; }
    }
}