using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Customers.Core.Clients.External.DTO;

namespace Inflow.Modules.Customers.Core.Clients;

internal interface IUserApiClient
{
    Task<UserDto> GetAsync(string email, CancellationToken cancellationToken = default);
}