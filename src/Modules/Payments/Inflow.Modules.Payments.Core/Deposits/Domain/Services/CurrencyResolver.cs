using Inflow.Shared.Abstractions.Kernel.ValueObjects;

namespace Inflow.Modules.Payments.Core.Deposits.Domain.Services;

internal sealed class CurrencyResolver : ICurrencyResolver
{
    public Currency GetForNationality(Nationality nationality)
        => nationality.Value switch
        {
            "PL" => "PLN",
            "DE" => "EUR",
            "FR" => "EUR",
            "ES" => "EUR",
            "GB" => "GBP",
            "MD" => "MDL",
            "US" => "USD",
            "CA" => "CAN",
            "RO" => "RON",
            _ => "EUR"
        };
}