using Inflow.Modules.Wallets.Core.Wallets.Entities;

namespace Inflow.Modules.Wallets.Application.Wallets.DTO;

internal class TransferDetailsDto : TransferDto
{
    public string Metadata { get; set; }
}