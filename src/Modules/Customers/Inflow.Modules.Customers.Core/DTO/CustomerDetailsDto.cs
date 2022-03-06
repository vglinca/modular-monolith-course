namespace Inflow.Modules.Customers.Core.DTO;

internal class CustomerDetailsDto : CustomerDto
{
    public IdentityDto Identity { get; set; }
    public string Address { get; set; }
    public string Notes { get; set; }
}