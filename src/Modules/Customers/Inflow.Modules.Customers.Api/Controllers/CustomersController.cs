using System;
using System.Threading.Tasks;
using Inflow.Modules.Customers.Core.Commands;
using Inflow.Modules.Customers.Core.DTO;
using Inflow.Modules.Customers.Core.Queries;
using Inflow.Shared.Abstractions.Dispatchers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Customers.Api.Controllers;

[ApiController]
[Route("[controller]")]
internal class CustomersController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public CustomersController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDetailsDto>> GetById(Guid id)
    {
        var customer = await _dispatcher.DispatchQueryAsync(new GetCustomer(id));
        
        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateCustomer command)
    {
        await _dispatcher.DispatchCommandAsync(command);
        return StatusCode(StatusCodes.Status201Created);
    }
}