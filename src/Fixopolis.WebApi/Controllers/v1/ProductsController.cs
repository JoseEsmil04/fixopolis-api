using Fixopolis.Application.Products.Commands;
using Fixopolis.Application.Products.Dtos;
using Fixopolis.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await mediator.Send(new GetProductsQuery()));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await mediator.Send(new GetProductByIdQuery(id));
        return product is not null ? Ok(product) : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand createProductCommand)
    {
        try
        {
            var id = await mediator.Send(createProductCommand);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
        {
            return Conflict(new { message = "El código de producto ya está en uso." });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            var command = new UpdateProductCommand(
                id,
                request.Name,
                request.Code,
                request.CategoryName,
                request.Description,
                request.Price,
                request.Stock,
                request.IsAvailable
            );

            var ok = await mediator.Send(command);
            return ok ? NoContent() : NotFound();

        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
        {
            return Conflict(new { message = "El código de producto ya está en uso." });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }



    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await mediator.Send(new DeleteProductCommand(id));
        return ok ? NoContent() : NotFound();
    }
}
