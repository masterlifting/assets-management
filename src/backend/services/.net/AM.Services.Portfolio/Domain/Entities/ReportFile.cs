using AM.Services.Portfolio.Domain.Entities.Catalogs;

using Shared.Contracts.Abstractions.Domains.Entities;
using Shared.Contracts.Abstractions.Domains.Entities.States;

using System;
using System.ComponentModel.DataAnnotations;
using static AM.Services.Portfolio.Enums;
using static Shared.Contracts.Enums;

namespace AM.Services.Portfolio.Domain.Entities;

public class ReportFile : IEntityState
{
    public long Id { get; set; }

    [Required, StringLength(200, MinimumLength = 3)]
    public string Name { get; set; } = null!;
    
    [Required, StringLength(30, MinimumLength = 2)]
    public string ContentType { get; set; } = null!;
    public byte[] Payload { get; set; } = null!;
    
    public virtual Provider Provider { get; init; } = null!;
    public int ProviderId { get; init; }
    public virtual User User { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public Report? Report { get; set; }
    
    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    public byte StateId { get; set; } = (byte) States.Ready;
    public virtual State State { get; set; } = null!;
    public byte StepId { get; set; } = (byte) Steps.Parsing;
    public virtual Step Step { get; set; } = null!;
    public int Attempt { get; set; }
    public string? Info { get; set; }
}