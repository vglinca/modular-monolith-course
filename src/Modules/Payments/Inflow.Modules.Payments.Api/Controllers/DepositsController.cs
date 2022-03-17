using System;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Deposits.Commands;
using Inflow.Modules.Payments.Core.Deposits.DTO;
using Inflow.Modules.Payments.Core.Deposits.Queries;
using Inflow.Shared.Abstractions.Contexts;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Payments.Api.Controllers;

[Route("[controller]")]
internal class DepositsController : BaseController
{
    private const string Policy = "deposits";
    private readonly IContext _context;
    public DepositsController(IDispatcher dispatcher, IContext context) : base(dispatcher) => _context = context;

    [HttpGet]
    [Authorize]
    public Task<ActionResult<PagedResult<DepositDto>>> Browse([FromQuery] GetDepositsPaged query)
        => ExecuteQuery<GetDepositsPaged, PagedResult<DepositDto>>(query);

    [HttpPost]
    [Authorize]
    public Task<ActionResult> Start(StartDeposit command)
        => ExecuteCommand(command);

    [HttpPut("{depositId:guid}/complete")]
    public Task<ActionResult> Complete(Guid depositId, CompleteDeposit command)
        => ExecuteCommand(command.Bind(x => x.DepositId, depositId));
}