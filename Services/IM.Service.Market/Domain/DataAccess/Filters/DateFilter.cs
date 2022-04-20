﻿using IM.Service.Common.Net.Models.Entity.Interfaces;
using IM.Service.Common.Net.RepositoryService.Filters;
using IM.Service.Market.Domain.Entities.Interfaces;

using static IM.Service.Common.Net.Enums;
using static IM.Service.Common.Net.Helper.ExpressionHelper;

namespace IM.Service.Market.Domain.DataAccess.Filters;

public class DateFilter<T> : FilterByDate<T> where T : class, IDateIdentity, IDataIdentity
{
    private string[] CompanyIds { get; } = Array.Empty<string>();
    private string? CompanyId { get; }
    private byte? SourceId { get; }

    public DateFilter(CompareType compareType, int year) : base(compareType, year) { }
    public DateFilter(CompareType compareType, int year, int month) : base(compareType, year, month) { }
    public DateFilter(CompareType compareType, int year, int month, int day) : base(compareType, year, month, day) { }

    public DateFilter(CompareType compareType, string companyId, int year) : base(compareType, year)
    {
        CompanyId = companyId.Trim().ToUpperInvariant();
        Expression = Combine(x => CompanyId == x.CompanyId, Expression);
    }
    public DateFilter(CompareType compareType, string companyId, int year, int month) : base(compareType, year, month)
    {
        CompanyId = companyId.Trim().ToUpperInvariant();
        Expression = Combine(x => CompanyId == x.CompanyId, Expression);
    }
    public DateFilter(CompareType compareType, string companyId, int year, int month, int day) : base(compareType, year, month, day)
    {
        CompanyId = companyId.Trim().ToUpperInvariant();
        Expression = Combine(x => CompanyId == x.CompanyId, Expression);
    }

    public DateFilter(CompareType compareType, byte sourceId, int year) : base(compareType, year)
    {
        SourceId = sourceId;
        Expression = Combine(x => sourceId == x.SourceId, Expression);
    }
    public DateFilter(CompareType compareType, byte sourceId, int year, int month) : base(compareType, year, month)
    {
        SourceId = sourceId;
        Expression = Combine(x => sourceId == x.SourceId, Expression);
    }
    public DateFilter(CompareType compareType, byte sourceId, int year, int month, int day) : base(compareType, year, month, day)
    {
        SourceId = sourceId;
        Expression = Combine(x => sourceId == x.SourceId, Expression);
    }

    public DateFilter(CompareType compareType, string companyId, byte sourceId, int year) : base(compareType, year)
    {
        CompanyId = companyId.Trim().ToUpperInvariant();
        SourceId = sourceId;
        Expression = Combine(x => CompanyId == x.CompanyId && SourceId == x.SourceId, Expression);
    }
    public DateFilter(CompareType compareType, string companyId, byte sourceId, int year, int month) : base(compareType, year, month)
    {
        CompanyId = companyId.Trim().ToUpperInvariant();
        SourceId = sourceId;
        Expression = Combine(x => CompanyId == x.CompanyId && SourceId == x.SourceId, Expression);
    }
    public DateFilter(CompareType compareType, string companyId, byte sourceId, int year, int month, int day) : base(compareType, year, month, day)
    {
        CompanyId = companyId.Trim().ToUpperInvariant();
        SourceId = sourceId;
        Expression = Combine(x => companyId == x.CompanyId && SourceId == x.SourceId, Expression);
    }

    public DateFilter(CompareType compareType, IEnumerable<string> companyIds, int year) : base(compareType, year)
    {
        CompanyIds = companyIds.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim().ToUpperInvariant()).ToArray();
        Expression = Combine(x => CompanyIds.Contains(x.CompanyId), Expression);
    }
    public DateFilter(CompareType compareType, IEnumerable<string> companyIds, int year, int month) : base(compareType, year, month)
    {
        CompanyIds = companyIds.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim().ToUpperInvariant()).ToArray();
        Expression = Combine(x => CompanyIds.Contains(x.CompanyId), Expression);
    }
    public DateFilter(CompareType compareType, IEnumerable<string> companyIds, int year, int month, int day) : base(compareType, year, month, day)
    {
        CompanyIds = companyIds.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim().ToUpperInvariant()).ToArray();
        Expression = Combine(x => CompanyIds.Contains(x.CompanyId), Expression);
    }
    public DateFilter(CompareType compareType, IEnumerable<string> companyIds, byte sourceId, int year) : base(compareType, year)
    {
        CompanyIds = companyIds.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim().ToUpperInvariant()).ToArray();
        SourceId = sourceId;
        Expression = Combine(x => CompanyIds.Contains(x.CompanyId) && SourceId == x.SourceId, Expression);
    }
    public DateFilter(CompareType compareType, IEnumerable<string> companyIds, byte sourceId, int year, int month) : base(compareType, year, month)
    {
        CompanyIds = companyIds.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim().ToUpperInvariant()).ToArray();
        SourceId = sourceId;
        Expression = Combine(x => CompanyIds.Contains(x.CompanyId) && SourceId == x.SourceId, Expression);
    }
    public DateFilter(CompareType compareType, IEnumerable<string> companyIds, byte sourceId, int year, int month, int day) : base(compareType, year, month, day)
    {
        CompanyIds = companyIds.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim().ToUpperInvariant()).ToArray();
        SourceId = sourceId;
        Expression = Combine(x => CompanyIds.Contains(x.CompanyId) && SourceId == x.SourceId, Expression);
    }
}