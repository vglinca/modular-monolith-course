using Inflow.Modules.Customers.Core.Domain.Entities;

namespace Inflow.Modules.Customers.Core.DTO;

internal static class Extensions
{
    public static CustomerDto AsDto(this Customer customer)
        => customer.Map<CustomerDto>();

    public static CustomerDetailsDto AsDetailsDto(this Customer customer)
    {
        var dto = customer.Map<CustomerDetailsDto>();
        dto.Address = customer.Address;
        dto.Notes = customer.Notes;
        dto.Identity = customer.Identity is null
            ? null
            : new IdentityDto()
            {
                Type = customer.Identity.Type,
                Series = customer.Identity.Series
            };

        return dto;
    }

    private static T Map<T>(this Customer customer) where T : CustomerDto, new()
        => new()
        {
            Id = customer.Id,
            Email = customer.Email,
            Name = customer.Name,
            Nationality = customer.Name,
            CreatedAt = customer.CreatedAt,
            FullName = customer.FullName,
            IsActive = customer.IsActive
        };
}