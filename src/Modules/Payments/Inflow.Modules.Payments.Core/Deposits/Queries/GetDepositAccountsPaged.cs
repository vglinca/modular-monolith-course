using System;
using Inflow.Modules.Payments.Core.Deposits.DTO;
using Inflow.Shared.Abstractions.Queries;

namespace Inflow.Modules.Payments.Core.Deposits.Queries;

internal class GetDepositAccountsPaged : PagedQuery<DepositAccountDto>
{
    public Guid? CustomerId { get; set; }
    public string Currency { get; set; }
}