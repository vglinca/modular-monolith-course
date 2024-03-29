using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Deposits.Domain.Entities;

namespace Inflow.Modules.Payments.Core.Deposits.Domain.Repositories;

internal interface IDepositRepository
{
    Task<Deposit> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Deposit deposit);
    Task UpdateAsync(Deposit deposit);
}