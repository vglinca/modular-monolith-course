using Inflow.Shared.Abstractions.Exceptions;

namespace Inflow.Modules.Users.Core.Exceptions;

internal class InvalidEmailProviderException : InflowException
{
    public string Email { get; }

    public InvalidEmailProviderException(string email) : base($"State is invalid: '{email}'.") => Email = email;
}