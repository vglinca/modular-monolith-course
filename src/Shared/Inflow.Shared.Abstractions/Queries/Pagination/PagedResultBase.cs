namespace Inflow.Shared.Abstractions.Queries;

public class PagedResultBase
{
    public int CurrentPage { get; set; }
    public int ResultsPerPage { get; set; }
    public int TotalPages { get; set; }
    public long TotalResults { get; set; }

    protected PagedResultBase() { }

    protected PagedResultBase(int currentPage, int resultsPerPage, int totalPages, long totalResults)
    {
        CurrentPage = currentPage;
        ResultsPerPage = resultsPerPage;
        TotalPages = totalPages;
        TotalResults = totalResults;
    }
}