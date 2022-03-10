namespace Inflow.Shared.Abstractions.Exceptions;

public abstract class ResourceNotFoundException : InflowException
{
    protected ResourceNotFoundException(string message) : base(message)
    {
    }
}