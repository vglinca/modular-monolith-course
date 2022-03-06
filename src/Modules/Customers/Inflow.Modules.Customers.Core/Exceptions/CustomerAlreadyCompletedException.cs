using System;
using Inflow.Shared.Abstractions.Exceptions;

namespace Inflow.Modules.Customers.Core.Exceptions;

internal class CustomerAlreadyCompletedException : InflowException
{
    public Guid Id { get; }

    public CustomerAlreadyCompletedException(Guid id) : base($"Customer with id '{id}' has already been completed.") => Id = id;
}