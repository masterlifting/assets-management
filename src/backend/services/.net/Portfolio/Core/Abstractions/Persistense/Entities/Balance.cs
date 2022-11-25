﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AM.Services.Portfolio.Core.Abstractions.Persistense.Entities;

public abstract class Balance : IBalance
{
    [Column(TypeName = "Decimal(18,10)")]
    public decimal BalanceCost { get; set; }

    [Column(TypeName = "Decimal(18,10)")]
    public decimal LastDealCost { get; set; }

    [Column(TypeName = "Decimal(18,10)")]
    public decimal BalanceValue { get; set; }
}