using Inflow.Shared.Abstractions.Exceptions;

namespace Inflow.Modules.Wallets.Core.Wallets.Exceptions;

public class InvalidTransferCurrencyException : BadRequestException
{
    public string Currency { get; }

    public InvalidTransferCurrencyException(string currency) : base($"Transfer has invalid currency: '{currency}'.")
    {
        Currency = currency;
    }
}