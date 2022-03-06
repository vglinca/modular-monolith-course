using System;
using System.Threading.Tasks;
using Inflow.Modules.Customers.Core.Domain.Entities;
using Inflow.Modules.Customers.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Customers.Core.DAL.Repositories;

internal class CustomersRepository : ICustomerRepository
{
    private readonly CustomersDbContext _ctx;

    public CustomersRepository(CustomersDbContext ctx) => _ctx = ctx;

    public Task<Customer> GetAsync(Guid id)
        => _ctx.Customers.SingleOrDefaultAsync(x => x.Id == id);

    public Task<bool> ExistsAsync(string name)
        => _ctx.Customers.AnyAsync(x => x.Name.Equals(name));

    public async Task AddAsync(Customer customer)
    {
        await _ctx.Customers.AddAsync(customer);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _ctx.Customers.Update(customer);
        await _ctx.SaveChangesAsync();
    }
}