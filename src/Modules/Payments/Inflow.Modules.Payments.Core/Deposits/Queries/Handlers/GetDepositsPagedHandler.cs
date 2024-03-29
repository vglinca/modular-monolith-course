using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.DAL;
using Inflow.Modules.Payments.Core.Deposits.Domain.Entities;
using Inflow.Modules.Payments.Core.Deposits.DTO;
using Inflow.Shared.Abstractions.Kernel.ValueObjects;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Payments.Core.Deposits.Queries.Handlers;

internal sealed class GetDepositsPagedHandler : IQueryHandler<GetDepositsPaged, PagedResult<DepositDto>>
{
    private readonly PaymentsDbContext _ctx;

    public GetDepositsPagedHandler(PaymentsDbContext ctx) => _ctx = ctx;

    public Task<PagedResult<DepositDto>> HandleAsync(GetDepositsPaged query, CancellationToken cancellationToken = default)
    {
        var deposits = _ctx.Deposits.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Currency))
        {
            _ = new Currency(query.Currency);
            deposits = deposits.Where(x => x.Currency.Equals(query.Currency));
        }

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<DepositStatus>(query.Status, true, out var status))
        {
            deposits = deposits.Where(x => x.Status == status);
        }

        if (query.AccountId.HasValue)
        {
            deposits = deposits.Where(x => x.AccountId == query.AccountId &&
                !query.CustomerId.HasValue || x.Account.CustomerId == query.CustomerId);
        }

        if (query.CustomerId.HasValue)
        {
            deposits = deposits.Where(x => x.Account.CustomerId == query.CustomerId);
        }

        return deposits.AsNoTracking()
            .Include(x => x.Account)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new DepositDto
            {
                DepositId = x.Id,
                Amount = x.Amount,
                Currency = x.Currency,
                Status = x.Status.ToString().ToLowerInvariant(),
                AccountId = x.AccountId,
                CustomerId = x.Account.CustomerId,
                CreatedAt = x.CreatedAt,
                ProcessedAt = x.ProcessedAt
            })
            .PaginateAsync(query, cancellationToken);
    }
}