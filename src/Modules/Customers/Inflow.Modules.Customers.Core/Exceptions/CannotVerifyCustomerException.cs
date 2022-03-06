using System;
using Inflow.Shared.Abstractions.Exceptions;

namespace Inflow.Modules.Customers.Core.Exceptions;

internal class CannotVerifyCustomerException : InflowException
{
    public Guid Id { get; }

    public CannotVerifyCustomerException(Guid id) : base($"Can't verify customer with id '{id}'") => Id = id;
}