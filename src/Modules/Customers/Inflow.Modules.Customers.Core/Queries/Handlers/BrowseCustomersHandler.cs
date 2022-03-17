using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Customers.Core.DAL;
using Inflow.Modules.Customers.Core.DTO;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Customers.Core.Queries.Handlers;

internal sealed class BrowseCustomersHandler : IQueryHandler<BrowseCustomers, PagedResult<CustomerDto>>
{
    private readonly CustomersDbContext _ctx;

    public BrowseCustomersHandler(CustomersDbContext ctx) => _ctx = ctx;

    public async Task<PagedResult<CustomerDto>> HandleAsync(BrowseCustomers query, CancellationToken cancellationToken = default)
    {
        var customersQuery = _ctx.Customers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.State))
        {
            var state = query.State.ToLowerInvariant();
            customersQuery = state switch
            {
                "new" => customersQuery.Where(x => !x.CompletedAt.HasValue && !x.VerifiedAt.HasValue),
                "completed" => customersQuery.Where(x => x.CompletedAt.HasValue),
                "verified" => customersQuery.Where(x => x.VerifiedAt.HasValue),
                "locked" => customersQuery.Where(x => !x.IsActive),
                _ => customersQuery
            };
        }

        return await customersQuery
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.AsDto())
            .PaginateAsync(query, cancellationToken);
    }
}