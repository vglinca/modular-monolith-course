using System;
using Inflow.Shared.Abstractions.Exceptions;

namespace Inflow.Modules.Customers.Core.Exceptions;

internal class CustomerAlreadyExistsException : BadRequestException
{
    public Guid Id { get; }

    public CustomerAlreadyExistsException(Guid id) : base($"Customer with id '{id}' already exists") => Id = id;
}