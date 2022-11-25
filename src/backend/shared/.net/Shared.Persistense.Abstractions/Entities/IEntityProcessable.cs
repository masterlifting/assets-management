﻿namespace Shared.Persistense.Abstractions.Entities;

public interface IEntityProcessable : IEntity
{
    Guid Id { get; init; }
    int ProcessStatusId { get; set; }
    int ProcessStepId { get; set; }
    byte ProcessAttempt { get; set; }
    DateTime Updated { get; set; }
}