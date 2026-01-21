using Fixopolis.Application.Orders.Commands.CancelOrder;
using Fixopolis.Application.Orders.Commands.CreateOrder;
using Fixopolis.Application.Orders.Commands.MarkOrderPaid;
using Fixopolis.Application.Orders.Dtos;
using Fixopolis.Application.Orders.Queries.GetOrderById;
using Fixopolis.Application.Orders.Queries.GetOrders;
using Fixopolis.Application.Orders.Queries.GetOrdersByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.WebApi.Controllers.v1;


[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrdersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("user/{userId:guid}")]
    [Authorize(Roles = "Admin,Employee,Customer")]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderDto>>> GetByUser(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new GetOrdersByUserQuery(userId), cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }


    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Employee,Customer")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderDto>> Create([FromBody] OrderCreateDto body, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _mediator.Send(new CreateOrderCommand(body), cancellationToken);
            var version = HttpContext.GetRequestedApiVersion()?.ToString();
            return CreatedAtAction(nameof(GetById), new { id = created.Id, version }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (DbUpdateException)
        {
            return Conflict();
        }
    }

    [HttpPost("{id:guid}/pay")]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> MarkPaid(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _mediator.Send(new MarkOrderPaidCommand(id), cancellationToken);
            if (!success)
            {
                return NotFound();
            }
            return Ok("Orden pagada con éxito.");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var success = await _mediator.Send(new CancelOrderCommand(id), cancellationToken);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }
}