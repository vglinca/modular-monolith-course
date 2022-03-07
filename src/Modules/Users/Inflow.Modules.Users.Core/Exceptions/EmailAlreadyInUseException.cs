using Inflow.Shared.Abstractions.Exceptions;

namespace Inflow.Modules.Users.Core.Exceptions;

internal class EmailAlreadyInUseException : InflowException
{
    public EmailAlreadyInUseException() : base("This email is already in use")
    {
    }
}