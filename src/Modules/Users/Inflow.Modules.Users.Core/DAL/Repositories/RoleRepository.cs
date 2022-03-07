using System.Collections.Generic;
using System.Threading.Tasks;
using Inflow.Modules.Users.Core.Entities;
using Inflow.Modules.Users.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Users.Core.DAL.Repositories;

internal class RoleRepository : IRoleRepository
{
    private readonly UsersDbContext _ctx;
    public RoleRepository(UsersDbContext ctx) => _ctx = ctx;

    public Task<Role> GetAsync(string name) => _ctx.Roles.SingleOrDefaultAsync(x => x.Name.Equals(name));
    public async Task<IReadOnlyList<Role>> GetAllAsync() => await _ctx.Roles.ToListAsync();

    public async Task AddAsync(Role role)
    {
        await _ctx.Roles.AddAsync(role);
        await _ctx.SaveChangesAsync();
    }
}