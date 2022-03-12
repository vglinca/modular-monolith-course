using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Deposits.DTO;
using Inflow.Modules.Payments.Core.Deposits.Queries;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Payments.Api.Controllers;

[Route("deposits/accounts")]
internal class DepositAccountsController : BaseController
{
    public DepositAccountsController(IDispatcher dispatcher) : base(dispatcher) { }

    [HttpGet]
    public Task<ActionResult<PagedResult<DepositAccountDto>>> Browse([FromQuery] GetDepositAccountsPaged query)
        => ExecuteQuery<GetDepositAccountsPaged, PagedResult<DepositAccountDto>>(query);
}