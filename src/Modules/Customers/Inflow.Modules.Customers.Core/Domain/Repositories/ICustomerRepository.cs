using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Customers.Core.Domain.Entities;

namespace Inflow.Modules.Customers.Core.Domain.Repositories;

internal interface ICustomerRepository
{
    Task<Customer> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string name);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
}