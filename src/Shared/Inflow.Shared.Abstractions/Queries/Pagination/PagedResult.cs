using System;
using System.Collections.Generic;
using System.Linq;

namespace Inflow.Shared.Abstractions.Queries;

public class PagedResult<T> : PagedResultBase
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

    public bool Empty => Items is null || !Items.Any();

    public PagedResult()
    {
        CurrentPage = 1;
        TotalPages = 1;
        ResultsPerPage = 10;
    }

    public PagedResult(IReadOnlyList<T> items, int currentPage, int resultsPerPage, int totalPages, long totalResults)
        : base(currentPage, resultsPerPage, totalPages, totalResults) => Items = items;

    public static PagedResult<T> Create(IReadOnlyList<T> items, int currentPage, int resultsPerPage, int totalPages,
        long totalResults) => new(items, currentPage, resultsPerPage, totalPages, totalResults);
    
    public static PagedResult<T> From(PagedResultBase result, IReadOnlyList<T> items)
        => new(items, result.CurrentPage, result.ResultsPerPage,
            result.TotalPages, result.TotalResults);

    public static PagedResult<T> AsEmpty => new();
    
    public PagedResult<TResult> Map<TResult>(Func<T, TResult> map)
    => PagedResult<TResult>.From(this, Items.Select(map).ToList());
}