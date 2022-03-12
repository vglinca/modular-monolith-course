using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Withdrawals.Commands;
using Inflow.Modules.Payments.Core.Withdrawals.DTO;
using Inflow.Modules.Payments.Core.Withdrawals.Queries;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Payments.Api.Controllers;

[Route("[controller]")]
internal class WithdrawalsController : BaseController
{
    public WithdrawalsController(IDispatcher dispatcher) : base(dispatcher) { }

    [HttpGet]
    public Task<ActionResult<PagedResult<WithdrawalDto>>> Browse([FromQuery] GetWithdrawalsPaged query)
        => ExecuteQuery<GetWithdrawalsPaged, PagedResult<WithdrawalDto>>(query);

    [HttpPost]
    public Task<ActionResult> Start(StartWithdrawal command)
        => ExecuteCommand(command);
}