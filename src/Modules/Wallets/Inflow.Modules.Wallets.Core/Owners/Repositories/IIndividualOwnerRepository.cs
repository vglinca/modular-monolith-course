using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Core.Owners.Entities;
using Inflow.Modules.Wallets.Core.Owners.Types;

namespace Inflow.Modules.Wallets.Core.Owners.Repositories;

internal interface IIndividualOwnerRepository
{
    Task<IndividualOwner> GetAsync(OwnerId id, CancellationToken cancellationToken = default);
    Task AddAsync(IndividualOwner owner);
    Task UpdateAsync(IndividualOwner owner);
}