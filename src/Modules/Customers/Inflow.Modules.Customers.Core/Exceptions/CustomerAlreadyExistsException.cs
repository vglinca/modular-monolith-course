using System;
using Inflow.Shared.Abstractions.Exceptions;

namespace Inflow.Modules.Users.Core.Exceptions;

internal class CustomerAlreadyExistsException : InflowException
{
    public Guid Id { get; }

    public CustomerAlreadyExistsException(Guid id) : base($"Customer with id '{id}' already exists") => Id = id;
}