using System;
using Inflow.Modules.Customers.Core.DTO;
using Inflow.Shared.Abstractions.Queries;

namespace Inflow.Modules.Customers.Core.Queries;

internal class GetCustomer : IQuery<CustomerDetailsDto>
{
    public GetCustomer(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}