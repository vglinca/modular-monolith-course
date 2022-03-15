using System;
using Inflow.Shared.Abstractions.Exceptions;

namespace Inflow.Modules.Wallets.Core.Wallets.Exceptions;

public class TransferNotFoundException : ResourceNotFoundException
{
    public Guid TransferId { get; }

    public TransferNotFoundException(Guid transferId) : base($"Transfer with ID: '{transferId}' was not found.")
    {
        TransferId = transferId;
    }
}