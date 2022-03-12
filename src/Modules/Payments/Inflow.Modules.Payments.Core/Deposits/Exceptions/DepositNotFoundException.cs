using System;
using Inflow.Shared.Abstractions.Exceptions;

namespace Inflow.Modules.Payments.Core.Deposits.Exceptions;

internal class DepositNotFoundException : ResourceNotFoundException
{
    public Guid DepositId { get; }

    public DepositNotFoundException(Guid depositId) : 
        base($"Deposit with id '{depositId}' was not found.") => DepositId = depositId;
}