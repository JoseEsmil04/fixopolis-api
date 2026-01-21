using Fixopolis.Application.Categories.Commands;
using Fixopolis.Application.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace Fixopolis.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CategoriesController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
        => Ok(await _mediator.Send(new GetCategoriesQuery()));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id));
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        try
        {
            var createdId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = createdId }, null);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sql &&
                  (sql.Number == 2601 || sql.Number == 2627))
        {
            return Conflict(new { message = "El nombre de categoría ya existe." });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand body)
    {
        try
        {
            var command = new UpdateCategoryCommand(id, body.Name);

            var ok = await _mediator.Send(command);
            return ok ? NoContent() : NotFound();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sql &&
                  (sql.Number == 2601 || sql.Number == 2627))
        {
            return Conflict(new { message = "El nombre de categoría ya existe." });
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
        await _mediator.Send(new DeleteCategoryCommand(id));
        return NoContent();
    }
}
