using System;
using System.Threading.Tasks;
using Inflow.Modules.Users.Core.Entities;
using Inflow.Modules.Users.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Users.Core.DAL.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly UsersDbContext _ctx;
    public UserRepository(UsersDbContext ctx) => _ctx = ctx;

    public Task<User> GetAsync(Guid id)
        => _ctx.Users.Include(x => x.Role)
            .SingleOrDefaultAsync(x => x.Id == id);

    public Task<User> GetAsync(string email)
        => _ctx.Users.Include(x => x.Role)
            .SingleOrDefaultAsync(x => x.Email.Equals(email));

    public async Task AddAsync(User user)
    {
        await _ctx.Users.AddAsync(user);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _ctx.Users.Update(user);
        await _ctx.SaveChangesAsync();
    }
}