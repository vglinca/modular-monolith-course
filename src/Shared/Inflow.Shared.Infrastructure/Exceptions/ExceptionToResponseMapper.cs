using System;
using System.Collections.Concurrent;
using System.Net;
using Humanizer;
using Inflow.Shared.Abstractions.Exceptions;

namespace Inflow.Shared.Infrastructure.Exceptions;

internal sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
{
    private static readonly ConcurrentDictionary<Type, string> Codes = new();

    public ExceptionResponse Map(Exception exception)
        => exception switch
        {
            ResourceNotFoundException e => new ExceptionResponse(
                new ErrorsResponse(new Error(GetErrorCode(e), e.Message)), HttpStatusCode.NotFound),
            BadRequestException e => new ExceptionResponse(new ErrorsResponse(new Error(GetErrorCode(e), e.Message)),
                HttpStatusCode.BadRequest),
            InflowException e => new ExceptionResponse(new ErrorsResponse(new Error(GetErrorCode(e), e.Message)),
                HttpStatusCode.BadRequest),
            _ => new ExceptionResponse(new ErrorsResponse(new Error("error", "There was an error")),
                HttpStatusCode.InternalServerError)
        };
    
    private record Error(string Code, string Message);
    private record ErrorsResponse(params Error[] Errors);

    private static string GetErrorCode(object exception)
    {
        var type = exception.GetType();
        return Codes.GetOrAdd(type, type.Name.Underscore().Replace("_exception", string.Empty));
    }
}