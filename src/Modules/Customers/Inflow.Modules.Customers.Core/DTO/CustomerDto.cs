using System;

namespace Inflow.Modules.Customers.Core.DTO;

internal class CustomerDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Name { get; set; }
    public string Nationality { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}