using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Models.ValueObject.EntityState;

namespace Shared.Persistense.Entities.EntityState;

public sealed class Step : Catalog, IEntityStepType
{
	public Step()
	{

	}
	public Step(StepId stepId) : base (stepId.AsInt, stepId.AsString)
	{

	}
}