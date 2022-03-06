using Inflow.Shared.Abstractions.Commands;

namespace Inflow.Modules.Customers.Core.Commands;

internal sealed record CreateCustomer(string Email) : ICommand;