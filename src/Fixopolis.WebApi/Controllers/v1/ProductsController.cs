using Fixopolis.Application.Products.Commands;
using Fixopolis.Application.Products.Queries;
using Fixopolis.Application.Products.Dtos;
using Fixopolis.Application.Products.Exceptions;
using Fixopolis.WebApi.Seed.Dtos;
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
public sealed class ProductsController(IMediator mediator, IProductImageStorage imageStorage) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int limit = 10, [FromQuery] int offset = 0, [FromQuery] string? query = null, [FromQuery] List<string>? categories = null)
        => Ok(await mediator.Send(new GetProductsQuery(new GetProductsDto 
        { 
            Limit = limit, 
            Offset = offset, 
            Query = query, 
            Categories = categories 
        })));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await mediator.Send(new GetProductByIdQuery(id));
        return product is not null ? Ok(product) : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Create([FromForm] CreateProductForm form)
    {
        try
        {
            string? imageUrl = null;
            
            // Si viene archivo, subir a Supabase
            if (form.ImageFile is not null)
            {
                imageUrl = await imageStorage.SaveImageAsync(form.ImageFile, HttpContext.RequestAborted);
            }
            // Si viene URL manual (supabase u otro), usarla directamente
            else if (!string.IsNullOrWhiteSpace(form.ImageUrl))
            {
                imageUrl = form.ImageUrl;
            }

            var command = new CreateProductCommand(
                form.Name,
                form.Code,
                form.Description,
                form.CategoryId,
                form.Price,
                form.Stock,
                form.IsAvailable,
                imageUrl,
                form.CategoryName);

            var id = await mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }
        catch (CategoryNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ProductCodeAlreadyExistsException)
        {
            return Conflict(new { message = "El código de producto ya está en uso." });
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
        {
            return Conflict(new { message = "El código de producto ya está en uso." });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromForm] UpdateProductForm form)
    {
        try
        {
            string? imageUrl = null;
            
            // Si viene archivo nuevo, subir a Supabase
            if (form.ImageFile is not null)
            {
                imageUrl = await imageStorage.SaveImageAsync(form.ImageFile, HttpContext.RequestAborted);
            }
            // Si viene URL manual, usarla
            else if (!string.IsNullOrWhiteSpace(form.ImageUrl))
            {
                imageUrl = form.ImageUrl;
            }

            var command = new UpdateProductCommand(
                id,
                form.Name,
                form.Code,
                form.CategoryId,
                form.Description,
                form.Price,
                form.Stock,
                form.IsAvailable,
                imageUrl,
                form.CategoryName);

            var ok = await mediator.Send(command);
            return ok ? NoContent() : NotFound();
        }
        catch (CategoryNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ProductCodeAlreadyExistsException)
        {
            return Conflict(new { message = "El código de producto ya está en uso." });
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
        {
            return Conflict(new { message = "El código de producto ya está en uso." });
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
