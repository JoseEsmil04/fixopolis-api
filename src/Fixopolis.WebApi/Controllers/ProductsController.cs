using Fixopolis.Application.Products.Commands;
using Fixopolis.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<IActionResult> Create([FromBody] CreateProductCommand cmd)
    {
        try
        {
            var id = await mediator.Send(cmd);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
        {
            return Conflict(new { message = "El código de producto ya está en uso." }); // 409
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand updateProductCommand)
    {
        if (id != updateProductCommand.Id) return BadRequest("El id de la ruta no coincide con el del cuerpo.");
        var ok = await mediator.Send(updateProductCommand);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await mediator.Send(new DeleteProductCommand(id));
        return ok ? NoContent() : NotFound();
    }
}
