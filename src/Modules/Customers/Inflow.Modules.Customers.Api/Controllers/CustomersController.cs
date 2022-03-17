using System;
using System.Threading.Tasks;
using Inflow.Modules.Customers.Core.Commands;
using Inflow.Modules.Customers.Core.DTO;
using Inflow.Modules.Customers.Core.Queries;
using Inflow.Shared.Abstractions.Contexts;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inflow.Modules.Customers.Api.Controllers;

[Route("[controller]")]
internal class CustomersController : BaseController
{
    private const string Policy = "customers";
    private readonly IContext _context;
    public CustomersController(IDispatcher dispatcher, IContext context) : base(dispatcher) => _context = context;

    [HttpGet("{id:guid}")]
    [Authorize]
    public Task<ActionResult<CustomerDetailsDto>> GetById(Guid id)
        => ExecuteQuery<GetCustomer, CustomerDetailsDto>(new GetCustomer(id));

    [HttpGet]
    [Authorize(Policy)]
    public Task<ActionResult<PagedResult<CustomerDto>>> GetPaged([FromQuery] BrowseCustomers query)
        => ExecuteQuery<BrowseCustomers, PagedResult<CustomerDto>>(query);

    [HttpPost]
    public Task<ActionResult> Create([FromBody] CreateCustomer command) => ExecuteCommandReturningCreated(command);

    [HttpPut("complete")]
    [Authorize]
    public Task<ActionResult> Complete([FromBody] CompleteCustomer command) 
        => ExecuteCommand(command.Bind(x => x.CustomerId, _context.IdentityContext.Id));

    [HttpPut("{customerId:guid}/verify")]
    [Authorize(Policy)]
    public Task<ActionResult> Verify([FromRoute] Guid customerId, VerifyCustomer command)
        => ExecuteCommand(command.Bind(x => x.CustomerId, customerId));

    [HttpPut("{customerId:guid}/lock")]
    [Authorize(Policy)]
    public Task<ActionResult> Lock([FromRoute] Guid customerId, LockCustomer command) 
        => ExecuteCommand(command.Bind(x => x.CustomerId, customerId));
    
    [HttpPut("{customerId:guid}/unlock")]
    [Authorize(Policy)]
    public Task<ActionResult> Unlock([FromRoute] Guid customerId, UnlockCustomer command) 
        => ExecuteCommand(command.Bind(x => x.CustomerId, customerId));
}