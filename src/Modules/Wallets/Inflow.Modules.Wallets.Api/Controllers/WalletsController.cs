using System;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Application.Wallets.DTO;
using Inflow.Modules.Wallets.Application.Wallets.Queries;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Wallets.Api.Controllers;

[Route("[controller]")]
internal class WalletsController : BaseController
{
    public WalletsController(IDispatcher dispatcher) : base(dispatcher)
    {
    }

    [HttpGet]
    public Task<ActionResult<PagedResult<WalletDto>>> GetPaged([FromQuery] BrowseWallets query)
        => ExecuteQuery<BrowseWallets, PagedResult<WalletDto>>(query);

    [HttpGet("{walletId:guid}")]
    public Task<ActionResult<WalletDetailsDto>> GetById([FromRoute] Guid walletId)
        => ExecuteQuery<GetWallet, WalletDetailsDto>(new GetWallet() {WalletId = walletId});
}