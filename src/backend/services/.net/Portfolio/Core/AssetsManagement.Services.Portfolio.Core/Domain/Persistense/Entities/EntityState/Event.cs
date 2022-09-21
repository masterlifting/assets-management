using AM.Services.Common.Contracts.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

using Shared.Persistense.Abstractions.Entities.EntityState;

using System.ComponentModel.DataAnnotations.Schema;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState
{
    public sealed class Event : SharedEntityState
    {

        [Column(TypeName = "Decimal(18,10)")]
        public decimal Value { get; set; }

        public DateOnly Date { get; set; }

        public EventType Type { get; set; } = null!;
        public int EventTypeId { get; set; } = (int)EventTypes.Default;

        public Derivative Derivative { get; set; } = null!;
        public string DerivativeId { get; set; } = null!;
        public string DerivativeCode { get; set; } = null!;

        public Account Account { get; set; } = null!;
        public int AccountId { get; set; }

        public User User { get; set; } = null!;
        public string UserId { get; set; } = null!;

        public Provider Provider { get; set; } = null!;
        public int ProviderId { get; set; }

        public Exchange Exchange { get; set; } = null!;
        public int ExchangeId { get; set; }
    }
}