using System;
using System.Linq.Expressions;
using AM.Services.Common.Contracts.Models.Entity.Interfaces;

namespace AM.Services.Common.Contracts.SqlAccess.Filters;

public interface IFilter<T> where T : IPeriod
{
    Expression<Func<T, bool>> Expression { get; set; }
}