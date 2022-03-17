using System;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Application.Wallets.Commands;
using Inflow.Modules.Wallets.Application.Wallets.DTO;
using Inflow.Modules.Wallets.Application.Wallets.Queries;
using Inflow.Shared.Abstractions.Contexts;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Wallets.Api.Controllers;

[Route("[controller]")]
[Authorize]
internal class TransfersController : BaseController
{
    private const string Policy = "transfers";
    private readonly IContext _context;
    public TransfersController(IDispatcher dispatcher, IContext context) : base(dispatcher) => _context = context;

    [HttpGet]
    [Authorize(Policy)]
    public Task<ActionResult<PagedResult<TransferDto>>> GetPaged([FromQuery] BrowseTransfers query)
        => ExecuteQuery<BrowseTransfers, PagedResult<TransferDto>>(query);

    [HttpGet("{transferId:guid}")]
    [Authorize(Policy)]
    public Task<ActionResult<TransferDetailsDto>> GetById([FromRoute] Guid transferId)
        => ExecuteQuery<GetTransfer, TransferDetailsDto>(new GetTransfer() {TransferId = transferId});

    [HttpPost("funds")]
    public Task<ActionResult> TransferFunds([FromBody] TransferFunds command)
        => ExecuteCommand(command.Bind(x => x.OwnerId, _context.IdentityContext.Id));

    [HttpPost("incoming")]
    [Authorize(Policy)]
    public Task<ActionResult> AddFunds([FromBody] AddFunds command)
        => ExecuteCommand(command);

    [HttpPost("outgoing")]
    [Authorize(Policy)]
    public Task<ActionResult> DeductFunds([FromBody] DeductFunds command)
        => ExecuteCommand(command);
}