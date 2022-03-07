using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Users.Core.DAL;
using Inflow.Modules.Users.Core.DTO;
using Inflow.Shared.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Users.Core.Queries.Handlers;

internal sealed class GetUserHandler : IQueryHandler<GetUser, UserDetailsDto>
{
    private readonly UsersDbContext _ctx;

    public GetUserHandler(UsersDbContext ctx) => _ctx = ctx;

    public async Task<UserDetailsDto> HandleAsync(GetUser query, CancellationToken cancellationToken = default)
    {
        var user = await _ctx.Users
            .AsNoTracking()
            .Include(x => x.Role)
            .SingleOrDefaultAsync(x => x.Id == query.UserId, cancellationToken);

        return user?.AsDetailsDto();
    }
}