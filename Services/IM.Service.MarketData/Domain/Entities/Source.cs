﻿using IM.Service.Common.Net.Models.Entity;
using IM.Service.MarketData.Domain.Entities.ManyToMany;

namespace IM.Service.MarketData.Domain.Entities;

public class Source : Catalog
{
    public virtual IEnumerable<CompanySource>? CompanySources { get; init; }

    public virtual IEnumerable<Price>? Prices { get; init; }
    public virtual IEnumerable<Report>? Reports { get; init; }
    public virtual IEnumerable<Coefficient>? Coefficients { get; init; }
    public virtual IEnumerable<Dividend>? Dividends { get; init; }
}