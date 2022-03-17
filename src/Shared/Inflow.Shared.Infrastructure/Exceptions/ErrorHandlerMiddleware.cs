using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Inflow.Shared.Infrastructure.Exceptions;

internal sealed class ErrorHandlerMiddleware : IMiddleware
{
    private readonly IExceptionCompositionRoot _exceptionCompositionRoot;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(IExceptionCompositionRoot exceptionCompositionRoot,
        ILogger<ErrorHandlerMiddleware> logger)
    {
        _exceptionCompositionRoot = exceptionCompositionRoot;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {

        try
        {
            await next(ctx);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            await HandleErrorAsync(ctx, e);
        }
    }

    private async Task HandleErrorAsync(HttpContext ctx, Exception ex)
    {
        var errorResponse = _exceptionCompositionRoot.Map(ex);
        ctx.Response.StatusCode = (int) (errorResponse?.StatusCode ?? HttpStatusCode.InternalServerError);
        var response = errorResponse?.Response;
        if (response is null)
        {
            return;
        }

        await ctx.Response.WriteAsJsonAsync(response);
    }
}