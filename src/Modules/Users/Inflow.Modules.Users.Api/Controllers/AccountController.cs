using System.Threading.Tasks;
using Inflow.Modules.Users.Core.Commands;
using Inflow.Modules.Users.Core.DTO;
using Inflow.Modules.Users.Core.Queries;
using Inflow.Modules.Users.Core.Services;
using Inflow.Shared.Abstractions.Dispatchers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Users.Api.Controllers;

internal class AccountController : BaseController
{
    private const string AccessTokenCookie = "__access-token";
    private readonly IDispatcher _dispatcher;
    private readonly IUserRequestStorage _userRequestStorage;
    private readonly CookieOptions _cookieOptions;

    public AccountController(IDispatcher dispatcher, IUserRequestStorage userRequestStorage, CookieOptions cookieOptions)
    {
        _dispatcher = dispatcher;
        _userRequestStorage = userRequestStorage;
        _cookieOptions = cookieOptions;
    }

    [HttpPost("sign-up")]
    public async Task<ActionResult> SignUp(SignUp command)
    {
        await _dispatcher.DispatchCommandAsync(command);
        return NoContent();
    }

    public async Task<ActionResult<UserDetailsDto>> SignIn(SignIn command)
    {
        await _dispatcher.DispatchCommandAsync(command);
        var jwt = _userRequestStorage.GetToken(command.Id);
        var user = await _dispatcher.DispatchQueryAsync(new GetUser {UserId = command.Id});
        AddCookie(AccessTokenCookie, jwt.AccessToken);
        return Ok(user);
    }
    
    private void AddCookie(string key, string value) => Response.Cookies.Append(key, value, _cookieOptions);
}