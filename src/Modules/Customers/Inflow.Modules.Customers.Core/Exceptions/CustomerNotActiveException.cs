using System;
using Inflow.Shared.Abstractions.Exceptions;

namespace Inflow.Modules.Customers.Core.Exceptions;

public class CustomerNotActiveException : InflowException
{
    public Guid Id { get; }

    public CustomerNotActiveException(Guid id) : base($"Customer with id {id} is inactive") => Id = id;
}