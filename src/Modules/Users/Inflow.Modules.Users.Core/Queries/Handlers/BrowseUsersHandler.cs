using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Users.Core.DAL;
using Inflow.Modules.Users.Core.DTO;
using Inflow.Modules.Users.Core.Entities;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Users.Core.Queries.Handlers;

internal sealed class BrowseUsersHandler : IQueryHandler<BrowseUsers, PagedResult<UserDto>>
{
    private readonly UsersDbContext _ctx;

    public BrowseUsersHandler(UsersDbContext ctx) => _ctx = ctx;

    public async Task<PagedResult<UserDto>> HandleAsync(BrowseUsers query, CancellationToken cancellationToken = default)
    {
        var usersQuery = _ctx.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Email))
        {
            usersQuery = usersQuery.Where(x => x.Email == query.Email);
        }

        if (!string.IsNullOrWhiteSpace(query.Role))
        {
            usersQuery = usersQuery.Where(x => x.RoleId == query.Role);
        }

        if (!string.IsNullOrWhiteSpace(query.State) && Enum.TryParse<UserState>(query.State, true, out var state))
        {
            usersQuery = usersQuery.Where(x => x.State == state);
        }

        return await usersQuery
            .AsNoTracking()
            .Include(x => x.Role)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.AsDto())
            .PaginateAsync(query, cancellationToken);
    }
}