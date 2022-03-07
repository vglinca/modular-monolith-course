using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Inflow.Shared.Infrastructure.Auth;

public class DisabledAuthenticationPolicyEvaluator : IPolicyEvaluator
{
    public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
    {
        var authenticationTicket = new AuthenticationTicket(new ClaimsPrincipal(), new AuthenticationProperties(),
            JwtBearerDefaults.AuthenticationScheme);
        return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
    }

    public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy,
        AuthenticateResult authenticationResult, HttpContext context,
        object resource) => Task.FromResult<PolicyAuthorizationResult>(PolicyAuthorizationResult.Success());
}