using System.Linq.Expressions;

namespace Shared.Models.Pagination;

public sealed class Paginator<T> where T : class
{
    private const int Max = 500;

    public int Page { get; }
    public int Limit { get; }

    public Paginator(int page, int limit)
    {
        Page = page <= 0 ? 1 : page >= int.MaxValue ? int.MaxValue : page;
        Limit = limit <= 0 ? Max : limit > Max ? limit == int.MaxValue ? int.MaxValue : Max : limit;
    }

    public IQueryable<T> GetAscPaginationResult<TSelector>(IQueryable<T> collection, Expression<Func<T, TSelector>> selector) =>
        collection.OrderBy(selector).Skip((Page - 1) * Limit).Take(Limit);
    public T[] GetAscPaginationResult<TSelector>(IEnumerable<T> collection, Func<T, TSelector> selector) =>
        collection.OrderBy(selector).Skip((Page - 1) * Limit).Take(Limit).ToArray();
    public IQueryable<T> GetDescPaginationResult<TSelector>(IQueryable<T> collection, Expression<Func<T, TSelector>> selector) =>
        collection.OrderByDescending(selector).Skip((Page - 1) * Limit).Take(Limit);
    public T[] GetDescPaginationResult<TSelector>(IEnumerable<T> collection, Func<T, TSelector> selector) =>
        collection.OrderByDescending(selector).Skip((Page - 1) * Limit).Take(Limit).ToArray();
    public IQueryable<T> GetAscPaginationResult<TSelector1, TSelector2>(IQueryable<T> collection, Expression<Func<T, TSelector1>> selector1, Expression<Func<T, TSelector2>> selector2) =>
        collection.OrderBy(selector1).ThenBy(selector2).Skip((Page - 1) * Limit).Take(Limit);
    public T[] GetAscPaginationResult<TSelector1, TSelector2>(IEnumerable<T> collection, Func<T, TSelector1> selector1, Func<T, TSelector2> selector2) =>
        collection.OrderBy(selector1).ThenBy(selector2).Skip((Page - 1) * Limit).Take(Limit).ToArray();
    public IQueryable<T> GetDescPaginationResult<TSelector1, TSelector2>(IQueryable<T> collection, Expression<Func<T, TSelector1>> selector1, Expression<Func<T, TSelector2>> selector2) =>
        collection.OrderByDescending(selector1).ThenByDescending(selector2).Skip((Page - 1) * Limit).Take(Limit);
    public T[] GetDescPaginationResult<TSelector1, TSelector2>(IEnumerable<T> collection, Func<T, TSelector1> selector1, Func<T, TSelector2> selector2) =>
        collection.OrderByDescending(selector1).ThenByDescending(selector2).Skip((Page - 1) * Limit).Take(Limit).ToArray();
}