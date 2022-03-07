using System.Collections.Generic;
using System.Threading.Tasks;
using Inflow.Modules.Users.Core.Entities;
using Inflow.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Users.Core.DAL;

internal sealed class UsersInitializer : IInitializer
{
    private readonly HashSet<string> _permissions = new()
    {
        "customers", "deposits", "withdrawals", "users", "transfers", "wallets"
    };

    private readonly UsersDbContext _ctx;
    private readonly ILogger<UsersInitializer> _logger;

    public UsersInitializer(UsersDbContext ctx, ILogger<UsersInitializer> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }

    public async Task InitAsync()
    {

        if (await _ctx.Roles.AnyAsync())
        {
            return;
        }
        
        await AddRolesAsync();
        await _ctx.SaveChangesAsync();
    }

    private async Task AddRolesAsync()
    {
        await _ctx.Roles.AddAsync(new Role
        {
            Name = "admin",
            Permissions = _permissions
        });
        
        await _ctx.Roles.AddAsync(new Role
        {
            Name = "user",
            Permissions = new List<string>()
        });

        _logger.LogInformation("Initialized roles.");
    }
}