﻿using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

using Shared.Persistense.Abstractions.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities;

public sealed class Expense : SharedEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; init; }

    public string DerivativeId { get; set; } = null!;
    public string DerivativeCode { get; set; } = null!;

    [JsonIgnore]
    public Deal Deal { get; set; } = null!;
    public string DealId { get; set; } = null!;

    [Column(TypeName = "Decimal(18,10)")]
    public decimal Value { get; set; }

    public DateOnly Date { get; set; }
}