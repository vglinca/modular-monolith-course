namespace Inflow.Shared.Abstractions.Exceptions;

public abstract class BadRequestException : InflowException
{
    protected BadRequestException(string message) : base(message)
    {
    }
}