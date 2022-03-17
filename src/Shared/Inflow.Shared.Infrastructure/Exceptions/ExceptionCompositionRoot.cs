using System;
using System.Linq;
using Inflow.Shared.Abstractions.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Shared.Infrastructure.Exceptions;

internal sealed class ExceptionCompositionRoot : IExceptionCompositionRoot
{
    private readonly IServiceProvider _serviceProvider;

    public ExceptionCompositionRoot(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public ExceptionResponse Map(Exception exception)
    {
        using var scope = _serviceProvider.CreateScope();
        var exceptionMappers = scope.ServiceProvider.GetServices<IExceptionToResponseMapper>().ToArray();
        var nonDefaultExceptionMappers = exceptionMappers
            .Where(x => x is not ExceptionToResponseMapper);

        var result = nonDefaultExceptionMappers
            .Select(x => x.Map(exception))
            .SingleOrDefault(x => x is not null);

        if (result is not null)
        {
            return result;
        }

        var defaultExceptionMapper = exceptionMappers.SingleOrDefault(x => x is ExceptionToResponseMapper);

        return defaultExceptionMapper?.Map(exception);
    }
}