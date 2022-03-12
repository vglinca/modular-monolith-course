using System;
using System.Threading.Tasks;
using Inflow.Modules.Customers.Core.Commands;
using Inflow.Modules.Customers.Core.DTO;
using Inflow.Modules.Customers.Core.Queries;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Customers.Api.Controllers;

[Route("[controller]")]
internal class CustomersController : BaseController
{
    public CustomersController(IDispatcher dispatcher) : base(dispatcher){}

    [HttpGet("{id:guid}")]
    public Task<ActionResult<CustomerDetailsDto>> GetById(Guid id)
        => ExecuteQuery<GetCustomer, CustomerDetailsDto>(new GetCustomer(id));

    [HttpPost]
    public Task<ActionResult> Create([FromBody] CreateCustomer command) => ExecuteCommandReturningCreated(command);

    [HttpPut("complete")]
    public Task<ActionResult> Complete([FromBody] CompleteCustomer command) => ExecuteCommand(command);

    [HttpPut("{customerId:guid}/verify")]
    public Task<ActionResult> Verify([FromRoute] Guid customerId) => ExecuteCommand(new VerifyCustomer(customerId));

    [HttpPut("lock")]
    public Task<ActionResult> Lock([FromBody] LockCustomer command) => ExecuteCommand(command);
    
    [HttpPut("{customerId:guid}/unlock")]
    public Task<ActionResult> Unlock([FromRoute] Guid customerId) => ExecuteCommand(new UnlockCustomer(customerId));
}