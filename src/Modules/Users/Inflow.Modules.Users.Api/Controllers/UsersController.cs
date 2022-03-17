using System;
using System.Data;
using System.Threading.Tasks;
using Inflow.Modules.Users.Core.Commands;
using Inflow.Modules.Users.Core.DTO;
using Inflow.Modules.Users.Core.Queries;
using Inflow.Shared.Abstractions.Contexts;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Users.Api.Controllers;

[Route("[controller]")]
[Authorize(Policy)]
internal class UsersController : BaseController
{
    private const string Policy = "users";
    private readonly IContext _context;
    public UsersController(IDispatcher dispatcher, IContext context) : base(dispatcher) => _context = context;

    [HttpGet]
    public Task<ActionResult<PagedResult<UserDto>>> GetPaged([FromQuery] BrowseUsers query)
        => ExecuteQuery<BrowseUsers, PagedResult<UserDto>>(query);

    [HttpGet("{userId:guid}")]
    public Task<ActionResult<UserDetailsDto>> GetById([FromRoute] Guid userId)
        => ExecuteQuery<GetUser, UserDetailsDto>(new GetUser() {UserId = userId});

    [HttpPut("{userId:guid}/state")]
    public Task<ActionResult> UpdateState([FromRoute] Guid userId, UpdateUserState command)
        => ExecuteCommand(command.Bind(x => x.UserId, userId));
}