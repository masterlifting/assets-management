using AM.Services.Common.Contracts.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using Shared.Persistense.Abstractions.Entities;

using System.ComponentModel.DataAnnotations.Schema;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities;

public sealed class Event : IPersistensableProcess
{
    [Column(TypeName = "Decimal(18,10)")]
    public decimal Value { get; set; }

    public DateOnly Date { get; set; }

    public EventType Type { get; set; } = null!;
    public int TypeId { get; set; }

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

    public DateTime Updated { get; set; }
    public string? Info { get; set; }
    public Guid Id { get; init; }
    public int ProcessStatusId { get; set; }
    public int ProcessStepId { get; set; }
    public byte ProcessAttempt { get; set; }
    public DateTime Created { get; init; }
}