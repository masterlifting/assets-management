using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

using Shared.Persistence.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Entities;

public sealed class Deal : IPersistentSql, IPersistentProcess
{
    public Guid Id { get; init; }

    public DateOnly Date { get; init; }
    public decimal Cost { get; init; }

    public ProcessStep ProcessStep { get; set; } = null!;
    public int ProcessStepId { get; set; }
    public ProcessStatus ProcessStatus { get; set; } = null!;
    public int ProcessStatusId { get; set; }
    public byte ProcessAttempt { get; set; }
    public string? Error { get; set; }

    public DateTime Created { get; init; }
    public DateTime Updated { get; set; }

    public Income Income { get; init; } = null!;
    public Expense Expense { get; init; } = null!;

    public User User { get; set; } = null!;
    public Guid UserId { get; set; }

    public Account Account { get; set; } = null!;
    public int AccountId { get; set; }

    public Provider Provider { get; set; } = null!;
    public int ProviderId { get; set; }

    public Exchange Exchange { get; set; } = null!;
    public int ExchangeId { get; set; }
    
    public string? Description { get; init; }
}