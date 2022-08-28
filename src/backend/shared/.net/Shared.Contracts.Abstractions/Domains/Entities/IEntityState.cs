using Shared.Contracts.Abstractions.Domains.Entities.States;

namespace Shared.Contracts.Abstractions.Domains.Entities;

public interface IEntityState
{
    public long Id { get; set; }

    public DateTime UpdateTime { get; set; }

    public byte StateId { get; set; }
    public State State { get; set; }
        
    public byte StepId { get; set; }
    public Step Step { get; set; }

    public int Attempt { get; set; }
        
    public string? Info { get; set; }
}