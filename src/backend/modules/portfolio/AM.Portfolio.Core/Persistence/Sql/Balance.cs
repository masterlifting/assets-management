using Net.Shared.Persistence.Abstractions.Entities;

namespace AM.Portfolio.Core.Persistence.Entities.Sql;

public class Balance : IPersistentSql, IPersistentProcess
{
    public int Id { get; init; }
    
    public decimal Value { get; set; }
    
    public Derivative Derivative { get; set; } = null!;
    public int DerivativeId { get; init; }
    
    public Guid? HostId { get; set; }
    public int StatusId { get; set; }
    public int StepId { get; set; }
    public byte Attempt { get; set; }
    public string? Error { get; set; }
    
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    public string? Description { get; set; }
}
