using System;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Application.Wallets.Commands;
using Inflow.Modules.Wallets.Application.Wallets.DTO;
using Inflow.Modules.Wallets.Application.Wallets.Queries;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Wallets.Api.Controllers;

[Route("[controller]")]
internal class TransfersController : BaseController
{
    public TransfersController(IDispatcher dispatcher) : base(dispatcher)
    {
    }

    [HttpGet]
    public Task<ActionResult<PagedResult<TransferDto>>> GetPaged([FromQuery] BrowseTransfers query)
        => ExecuteQuery<BrowseTransfers, PagedResult<TransferDto>>(query);

    [HttpGet("{transferId:guid}")]
    public Task<ActionResult<TransferDetailsDto>> GetById([FromRoute] Guid transferId)
        => ExecuteQuery<GetTransfer, TransferDetailsDto>(new GetTransfer() {TransferId = transferId});

    [HttpPost("funds")]
    public Task<ActionResult> TransferFunds([FromBody] TransferFunds command)
        => ExecuteCommand(command);

    [HttpPost("incoming")]
    public Task<ActionResult> AddFunds([FromBody] AddFunds command)
        => ExecuteCommand(command);

    [HttpPost("outgoing")]
    public Task<ActionResult> DeductFunds([FromBody] DeductFunds command)
        => ExecuteCommand(command);
}