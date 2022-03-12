using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Infrastructure.Entities;

[assembly: InternalsVisibleTo("Inflow.Modules.Payments.Core")]
namespace Inflow.Modules.Payments.Infrastructure.Repositories;

internal interface ICustomerRepository
{
    Task<Customer> GetAsync(Guid id);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
}