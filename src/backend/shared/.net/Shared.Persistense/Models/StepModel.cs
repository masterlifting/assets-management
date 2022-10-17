using Shared.Persistense.Entities.EntityState;
using Shared.Persistense.Models.ValueObject.EntityState;

namespace Shared.Persistense.Models;

public sealed record StepModel
{
	public StepModel(StepId stepId, string? info)
	{
        StepId = stepId;
        Info = info;
    }

    public StepId StepId { get; }
    public string? Info { get; }

    public Step GetEntity() => new()
    {
        Id = StepId.AsInt,
        Name = StepId.AsString,
        Info = Info,
        UpdateTime = DateTime.UtcNow
    };
}
