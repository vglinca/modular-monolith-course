using System;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Infrastructure.Entities;
using Inflow.Modules.Payments.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Payments.Core.DAL.Repositories;

internal class CustomerRepository : ICustomerRepository
{
    private readonly PaymentsDbContext _ctx;

    public CustomerRepository(PaymentsDbContext ctx) => _ctx = ctx;

    public Task<Customer> GetAsync(Guid id)
        => _ctx.Customers.SingleOrDefaultAsync(x => x.Id == id);

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