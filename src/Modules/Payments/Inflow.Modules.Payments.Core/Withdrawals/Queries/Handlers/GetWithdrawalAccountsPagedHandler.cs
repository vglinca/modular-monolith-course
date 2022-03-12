using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.DAL;
using Inflow.Modules.Payments.Core.Withdrawals.DTO;
using Inflow.Shared.Abstractions.Kernel.ValueObjects;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Payments.Core.Withdrawals.Queries.Handlers;

internal sealed class GetWithdrawalAccountsPagedHandler : 
    IQueryHandler<GetWithdrawalAccountsPaged, PagedResult<WithdrawalAccountDto>>
{
    private readonly PaymentsDbContext _ctx;

    public GetWithdrawalAccountsPagedHandler(PaymentsDbContext ctx) => _ctx = ctx;

    public Task<PagedResult<WithdrawalAccountDto>> HandleAsync(GetWithdrawalAccountsPaged query, 
        CancellationToken cancellationToken = default)
    {
        var accounts = _ctx.WithdrawalAccounts.AsQueryable();
            
        if (!string.IsNullOrWhiteSpace(query.Currency))
        {
            _ = new Currency(query.Currency);
            accounts = accounts.Where(x => x.Currency == query.Currency);
        }
            
        if (query.CustomerId.HasValue)
        {
            accounts = accounts.Where(x => x.CustomerId == query.CustomerId);
        }

        return accounts.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new WithdrawalAccountDto
            {
                AccountId = x.Id,
                CustomerId = x.CustomerId,
                Currency = x.Currency,
                Iban = x.Iban,
                CreatedAt = x.CreatedAt
            })
            .PaginateAsync(query, cancellationToken);
    }
}