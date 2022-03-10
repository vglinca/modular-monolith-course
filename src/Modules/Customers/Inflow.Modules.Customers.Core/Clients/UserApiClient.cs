using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Customers.Core.Clients.External.DTO;
using Inflow.Shared.Abstractions.Modules;

namespace Inflow.Modules.Customers.Core.Clients;

internal class UserApiClient : IUserApiClient
{
    private readonly IModuleClient _moduleClient;

    public UserApiClient(IModuleClient moduleClient)
    {
        _moduleClient = moduleClient;
    }

    public Task<UserDto> GetAsync(string email, CancellationToken cancellationToken = default)
        => _moduleClient.SendAsync<UserDto>("users/get-by-email", new {email}, cancellationToken);
}