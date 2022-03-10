using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inflow.Shared.Abstractions.Modules;

public interface IModuleSubscriber
{
    IModuleSubscriber Subscribe<TRequest, TResponse>(string path, Func<TRequest,
        IServiceProvider, CancellationToken, Task<TResponse>> callback) where TRequest : class where TResponse : class;
}