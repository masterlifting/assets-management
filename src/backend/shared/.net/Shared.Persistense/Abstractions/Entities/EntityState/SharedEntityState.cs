using Shared.Persistense.Entities.EntityState;

using System.ComponentModel.DataAnnotations;

namespace Shared.Persistense.Abstractions.Entities.EntityState
{
    public abstract class SharedEntityState : SharedEntity, IEntityState
    {
        [Key, StringLength(50)]
        public string Id { get; init; } = null!;

        public State State { get; set; } = null!;
        public int StateId { get; set; }

        public Step Step { get; init; } = null!;
        public int StepId { get; set; }

        public byte Attempt { get; set; }
    }
}