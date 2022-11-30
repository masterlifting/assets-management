using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

using Shared.Persistence.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Entities;

public sealed class Event : IPersistentProcess
{
    public Guid Id { get; init; }

    public DateOnly Date { get; set; }
    public decimal Value { get; set; }

    public ProcessStep ProcessStep { get; set; } = null!;
    public int ProcessStepId { get; set; }
    public ProcessStatus ProcessStatus { get; set; } = null!;
    public int ProcessStatusId { get; set; }
    public byte ProcessAttempt { get; set; }

    public DateTime Updated { get; set; }
    public DateTime Created { get; init; }
    public string? Info { get; set; }

    public User User { get; set; } = null!;
    public Guid UserId { get; set; }
    
    public Derivative Derivative { get; set; } = null!;
    public Guid DerivativeId { get; set; }

    public EventType Type { get; set; } = null!;
    public int TypeId { get; set; }

    public Account Account { get; set; } = null!;
    public int AccountId { get; set; }

    public Provider Provider { get; set; } = null!;
    public int ProviderId { get; set; }

    public Exchange Exchange { get; set; } = null!;
    public int ExchangeId { get; set; }
}