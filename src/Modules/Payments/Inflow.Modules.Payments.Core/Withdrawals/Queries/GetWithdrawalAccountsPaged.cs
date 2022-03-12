using System;
using Inflow.Modules.Payments.Core.Withdrawals.DTO;
using Inflow.Shared.Abstractions.Queries;

namespace Inflow.Modules.Payments.Core.Withdrawals.Queries;

internal class GetWithdrawalAccountsPaged : PagedQuery<WithdrawalAccountDto>
{
    public Guid? CustomerId { get; set; }
    public string Currency { get; set; }
}