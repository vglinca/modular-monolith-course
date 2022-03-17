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

[Route("[controller]")]
internal class WithdrawalsController : BaseController
{
    private const string Policy = "withdrawals";
    private readonly IContext _context;
    public WithdrawalsController(IDispatcher dispatcher, IContext context) : base(dispatcher) => _context = context;

    [HttpGet]
    [Authorize]
    public Task<ActionResult<PagedResult<WithdrawalDto>>> Browse([FromQuery] GetWithdrawalsPaged query)
    {
        if (query.CustomerId.HasValue || _context.IdentityContext.IsUser())
        {
            query.CustomerId = _context.IdentityContext.IsUser() ? _context.IdentityContext.Id : query.CustomerId;
        }
        
        return ExecuteQuery<GetWithdrawalsPaged, PagedResult<WithdrawalDto>>(query);
    }

    [HttpPost]
    [Authorize]
    public Task<ActionResult> Start(StartWithdrawal command)
        => ExecuteCommand(command.Bind(x => x.CustomerId, _context.IdentityContext.Id));
}