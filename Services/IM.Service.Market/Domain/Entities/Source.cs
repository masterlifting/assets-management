﻿using IM.Service.Common.Net.Models.Entity;
using IM.Service.Market.Domain.Entities.ManyToMany;

namespace IM.Service.Market.Domain.Entities;

public class Source : Catalog
{
    public virtual IEnumerable<CompanySource>? CompanySources { get; init; }

    public virtual IEnumerable<Price>? Prices { get; init; }
    public virtual IEnumerable<Report>? Reports { get; init; }
    public virtual IEnumerable<Coefficient>? Coefficients { get; init; }
    public virtual IEnumerable<Dividend>? Dividends { get; init; }
    public virtual IEnumerable<Float>? Floats { get; init; }
    public virtual IEnumerable<Split>? Splits { get; init; }
}