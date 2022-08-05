using System;
using System.Linq.Expressions;
using AM.Services.Common.Contracts.Models.Entity.Interfaces;
using static AM.Services.Common.Contracts.Enums;
using static AM.Services.Common.Contracts.Helpers.LogicHelper.PeriodConfigurator;

namespace AM.Services.Common.Contracts.SqlAccess.Filters;

public abstract class FilterByQuarter<T> : IFilter<T> where T : class, IQuarterIdentity
{
    private int Year { get; }
    private byte Quarter { get; }
    public Expression<Func<T, bool>> Expression { get; set; }

    protected FilterByQuarter(CompareType compareType, int year)
    {
        Year = SetYear(year);
        Quarter = 1;
        Expression = x => compareType == CompareType.More
            ? x.Year >= Year
            : x.Year == Year;
    }
    protected FilterByQuarter(CompareType compareType, int year, int quarter)
    {
        Year = SetYear(year);
        Quarter = SetQuarter(quarter);
        
        Expression = x => compareType == CompareType.More
            ? x.Year > Year || x.Year == Year && x.Quarter >= Quarter
            : x.Year == Year && x.Quarter == Quarter;
    }
}