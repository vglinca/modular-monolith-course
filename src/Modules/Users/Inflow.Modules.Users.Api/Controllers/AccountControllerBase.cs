using Inflow.Shared.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Users.Api.Controllers;

[ApiController]
[Route("[controller]")]
[ProducesDefaultContentType]
internal abstract class AccountControllerBase : ControllerBase
{
    protected ActionResult<T> OkOrNotFound<T>(T model)
    {
        if (model is not null)
        {
            return Ok(model);
        }

        return NotFound();
    }
}