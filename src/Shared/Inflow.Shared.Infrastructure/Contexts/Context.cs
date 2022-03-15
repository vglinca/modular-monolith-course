using System;
using Inflow.Shared.Abstractions.Contexts;
using Inflow.Shared.Infrastructure.Api;
using Microsoft.AspNetCore.Http;

namespace Inflow.Shared.Infrastructure.Contexts;

public class Context : IContext
{
    public Guid RequestId { get; } = Guid.NewGuid();
    
    // public Guid CorrelationId { get; }
    public string TraceId { get; }
    public string IpAddress { get; }
    public string UserAgent { get; }
    public IIdentityContext IdentityContext { get; }
    
    public Context() : this($"{Guid.NewGuid():N}", null)
    {
    }
    
    public Context(HttpContext context) : this(context.TraceIdentifier,
        new IdentityContext(context.User), context.GetUserIpAddress(),
        context.Request.Headers["user-agent"])
    {
    }
    
    public Context(/*Guid? correlationId, */string traceId, IIdentityContext identity = null, string ipAddress = null,
        string userAgent = null)
    {
        // CorrelationId = correlationId ?? Guid.NewGuid();
        TraceId = traceId;
        IdentityContext = identity ?? Contexts.IdentityContext.Empty;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }
    
    public static IContext Empty => new Context();
}