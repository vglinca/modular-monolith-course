using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Deposits.DTO;
using Inflow.Modules.Payments.Core.Deposits.Queries;
using Inflow.Shared.Abstractions.Contexts;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Payments.Api.Controllers;

[Route("deposits/accounts")]
internal class DepositAccountsController : BaseController
{
    private readonly IContext _context;
    public DepositAccountsController(IDispatcher dispatcher, IContext context) : base(dispatcher) => _context = context;

    [HttpGet]
    [Authorize]
    public Task<ActionResult<PagedResult<DepositAccountDto>>> Browse([FromQuery] GetDepositAccountsPaged query)
    {
        if (query.CustomerId.HasValue || _context.IdentityContext.IsUser())
        {
            query.CustomerId = _context.IdentityContext.IsUser() ? _context.IdentityContext.Id : query.CustomerId;
        }
        
        return ExecuteQuery<GetDepositAccountsPaged, PagedResult<DepositAccountDto>>(query);
    }
}