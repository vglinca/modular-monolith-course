using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Customers.Core.DAL;
using Inflow.Modules.Customers.Core.DTO;
using Inflow.Shared.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Customers.Core.Queries.Handlers;

internal sealed class GetCustomerHandler : IQueryHandler<GetCustomer, CustomerDetailsDto>
{
    private readonly CustomersDbContext _ctx;

    public GetCustomerHandler(CustomersDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<CustomerDetailsDto> HandleAsync(GetCustomer query, CancellationToken cancellationToken = default)
    {
        var customer = await _ctx.Customers
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == query.Id, cancellationToken);

        return customer?.AsDetailsDto();
    }
}