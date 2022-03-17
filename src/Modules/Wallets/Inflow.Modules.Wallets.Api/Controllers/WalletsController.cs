using System;
using System.Threading.Tasks;
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
internal class WalletsController : BaseController
{
    private const string Policy = "wallets";
    private readonly IContext _context;
    public WalletsController(IDispatcher dispatcher, IContext context) : base(dispatcher) => _context = context;

    [HttpGet]
    public Task<ActionResult<PagedResult<WalletDto>>> GetPaged([FromQuery] BrowseWallets query)
    {
        if (query.OwnerId.HasValue || _context.IdentityContext.IsUser())
        {
            query.OwnerId = _context.IdentityContext.IsUser() ? _context.IdentityContext.Id : query.OwnerId;
        }
        
        return ExecuteQuery<BrowseWallets, PagedResult<WalletDto>>(query);
    }

    [HttpGet("{walletId:guid}")]
    public Task<ActionResult<WalletDetailsDto>> GetById([FromRoute] Guid walletId)
        => ExecuteQuery<GetWallet, WalletDetailsDto>(new GetWallet() {WalletId = walletId});
}