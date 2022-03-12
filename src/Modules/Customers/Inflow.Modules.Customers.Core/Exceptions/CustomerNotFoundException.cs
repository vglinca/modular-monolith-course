using System;
using Inflow.Shared.Abstractions.Exceptions;

namespace Inflow.Modules.Customers.Core.Exceptions;

internal class CustomerNotFoundException : ResourceNotFoundException
{
    public Guid CustomerId { get; }

    public CustomerNotFoundException(Guid customerId) : base($"Customer with ID: '{customerId}' was not found.") 
        => CustomerId = customerId;
}