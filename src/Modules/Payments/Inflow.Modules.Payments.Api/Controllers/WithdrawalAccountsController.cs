using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Withdrawals.Commands;
using Inflow.Modules.Payments.Core.Withdrawals.DTO;
using Inflow.Modules.Payments.Core.Withdrawals.Queries;
using Inflow.Shared.Abstractions.Contexts;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Payments.Api.Controllers;

[Route("withdrawals/accounts")]
internal class WithdrawalAccountsController : BaseController
{
    private readonly IContext _context;
    public WithdrawalAccountsController(IDispatcher dispatcher, IContext context) : base(dispatcher) 
        => _context = context;

    [HttpGet]
    [Authorize]
    public Task<ActionResult<PagedResult<WithdrawalAccountDto>>> Browse([FromQuery] GetWithdrawalAccountsPaged query)
    {
        if (query.CustomerId.HasValue || _context.IdentityContext.IsUser())
        {
            query.CustomerId = _context.IdentityContext.IsUser() ? _context.IdentityContext.Id : query.CustomerId;
        }
        
        return ExecuteQuery<GetWithdrawalAccountsPaged, PagedResult<WithdrawalAccountDto>>(query);
    }

    [HttpPost]
    [Authorize]
    public Task<ActionResult> Create(AddWithdrawalAccount command)
        => ExecuteCommandReturningCreated(command.Bind(x => x.CustomerId, _context.IdentityContext.Id));
}