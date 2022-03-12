using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Withdrawals.Commands;
using Inflow.Modules.Payments.Core.Withdrawals.DTO;
using Inflow.Modules.Payments.Core.Withdrawals.Queries;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Payments.Api.Controllers;

[Route("withdrawals/accounts")]
internal class WithdrawalAccountsController : BaseController
{
    public WithdrawalAccountsController(IDispatcher dispatcher) : base(dispatcher) { }

    [HttpGet]
    public Task<ActionResult<PagedResult<WithdrawalAccountDto>>> Browse([FromQuery] GetWithdrawalAccountsPaged query)
        => ExecuteQuery<GetWithdrawalAccountsPaged, PagedResult<WithdrawalAccountDto>>(query);

    [HttpPost]
    public Task<ActionResult> Create(AddWithdrawalAccount command)
        => ExecuteCommandReturningCreated(command);
}