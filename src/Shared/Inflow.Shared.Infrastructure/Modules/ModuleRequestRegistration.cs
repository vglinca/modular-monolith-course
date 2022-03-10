using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inflow.Shared.Infrastructure.Modules;

internal class ModuleRequestRegistration
{
    public Type RequestType { get; }
    public Type ResponseType { get; set; }
    public Func<object, CancellationToken, Task<object>> Callback { get; }

    public ModuleRequestRegistration(Type requestType, Type responseType, Func<object, CancellationToken, Task<object>> callback)
    {
        RequestType = requestType;
        ResponseType = responseType;
        Callback = callback;
    }
}