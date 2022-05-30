using System;

namespace Inflow.Shared.Abstractions.Exceptions;

public class ResourceNotFoundException : InflowException
{
    public ResourceNotFoundException(string message) : base(message){}

    public static ResourceNotFoundException OfType<T>(Guid id) 
        => new($"{typeof(T).Name} with ID: '{id}' was not found");
    
    public static ResourceNotFoundException OfType<T>(object identifier, string identifierName)
        => new($"{typeof(T).Name} with {identifierName}: '{identifier.ToString()}' was not found");
}