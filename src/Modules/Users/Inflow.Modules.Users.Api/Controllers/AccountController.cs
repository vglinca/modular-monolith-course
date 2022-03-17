using System.Threading.Tasks;
using Inflow.Modules.Users.Core.Commands;
using Inflow.Modules.Users.Core.DTO;
using Inflow.Modules.Users.Core.Queries;
using Inflow.Modules.Users.Core.Services;
using Inflow.Shared.Abstractions.Contexts;
using Inflow.Shared.Abstractions.Dispatchers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Users.Api.Controllers;

internal class AccountController : AccountControllerBase
{
    private const string AccessTokenCookie = "__access-token";
    private readonly IDispatcher _dispatcher;
    private readonly IUserRequestStorage _userRequestStorage;
    private readonly CookieOptions _cookieOptions;
    private readonly IContext _context;

    public AccountController(IDispatcher dispatcher, IUserRequestStorage userRequestStorage, 
        CookieOptions cookieOptions, IContext context)
    {
        _dispatcher = dispatcher;
        _userRequestStorage = userRequestStorage;
        _cookieOptions = cookieOptions;
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<UserDetailsDto>> GetAccount()
        => OkOrNotFound(await _dispatcher.QueryAsync(new GetUser() {UserId = _context.IdentityContext.Id}));

    [HttpPost("sign-up")]
    public async Task<ActionResult> SignUp(SignUp command)
    {
        await _dispatcher.SendAsync(command);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("sign-in")]
    public async Task<ActionResult<UserDetailsDto>> SignIn(SignIn command)
    {
        await _dispatcher.SendAsync(command);
        var jwt = _userRequestStorage.GetToken(command.Id);
        var user = await _dispatcher.QueryAsync(new GetUser {UserId = jwt.UserId});
        AddCookie(AccessTokenCookie, jwt.AccessToken);
        return Ok(user);
    }

    [HttpDelete("sign-out")]
    [Authorize]
    public async Task<ActionResult> SignOutAsync()
    {
        await _dispatcher.SendAsync(new SignOut(_context.IdentityContext.Id));
        DeleteCookie(AccessTokenCookie);
        return NoContent();
    }

    private void AddCookie(string key, string value) => Response.Cookies.Append(key, value, _cookieOptions);
    private void DeleteCookie(string key) => Response.Cookies.Delete(key, _cookieOptions);
}