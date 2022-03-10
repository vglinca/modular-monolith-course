using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inflow.Shared.Infrastructure.Modules;

public class ModuleBroadcastRegistration
{
    public Type ReceiverType { get; }
    public Func<object, CancellationToken, Task> Callback { get; }
    public string Key => ReceiverType.Name;

    public ModuleBroadcastRegistration(Type receiverType, Func<object, CancellationToken, Task> callback)
    {
        ReceiverType = receiverType;
        Callback = callback;
    }
}