using System;
using System.Linq.Expressions;
using AM.Services.Common.Contracts.Models.Entity.Interfaces;
using static AM.Services.Common.Contracts.Enums;
using static AM.Services.Common.Contracts.Helpers.LogicHelper.PeriodConfigurator;

namespace AM.Services.Common.Contracts.SqlAccess.Filters;

public abstract class FilterByDate<T> : IFilter<T> where T : class, IDateIdentity
{
    private int Year { get; }
    private int Month { get; }
    private int Day { get; }
    public Expression<Func<T, bool>> Expression { get; set; }
    
    protected FilterByDate(CompareType compareType, int year)
    {
        Year = SetYear(year);
        Month = 1;
        Day = 1;

        Expression = x => compareType == CompareType.More
            ? x.Date.Year >= Year
            : x.Date.Year == Year;
    }
    protected FilterByDate(CompareType compareType, int year, int month)
    {
        Year = SetYear(year);
        Month = SetMonth(month);
        Day = 1;

        Expression = x => compareType == CompareType.More
            ? x.Date.Year > Year || x.Date.Year == Year && x.Date.Month >= Month
            : x.Date.Year == Year && x.Date.Month == Month;
    }
    protected FilterByDate(CompareType compareType, int year, int month, int day)
    {
        Year = SetYear(year);
        Month = SetMonth(month, compareType);
        Day = SetDay(day);

        Expression = x => compareType == CompareType.More
            ? x.Date.Year > Year || x.Date.Year == Year && (x.Date.Month == Month && x.Date.Day >= Day || x.Date.Month > Month)
            : x.Date.Year == Year && x.Date.Month == Month && x.Date.Day == Day;
    }
}