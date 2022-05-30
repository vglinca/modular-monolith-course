using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Deposits.Domain.Entities;
using Inflow.Shared.Abstractions.Kernel.ValueObjects;
using Microsoft.Extensions.Localization;

namespace Inflow.Modules.Payments.Core.Deposits.Domain.Repositories;

internal interface IDepositAccountRepository
{
    Task<DepositAccount> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DepositAccount> GetAsync(Guid customerId, Currency currency, CancellationToken cancellationToken = default);
    Task AddAsync(DepositAccount depositAccount);
}