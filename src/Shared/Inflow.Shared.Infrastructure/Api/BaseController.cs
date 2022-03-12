using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Shared.Infrastructure.Api;

[ApiController]
public abstract class BaseController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    protected BaseController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    protected async Task<ActionResult<TResult>> ExecuteQuery<TQuery, TResult>(TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        var result = await _dispatcher.QueryAsync(query, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }
        
        return Ok(result);
    }

    protected async Task<ActionResult> ExecuteCommand<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        await _dispatcher.SendAsync(command);
        return NoContent();
    }

    protected async Task<ActionResult> ExecuteCommandReturningCreated<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        await _dispatcher.SendAsync(command);
        return StatusCode(StatusCodes.Status201Created);
    }
}