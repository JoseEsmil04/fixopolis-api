using Fixopolis.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SeedController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Run(
        [FromServices] FixopolisDbContext db,
        [FromServices] IWebHostEnvironment env,
        [FromHeader(Name = "X-Seed-Token")] string? token,
        CancellationToken ct)
    {
        if (!env.IsDevelopment())
            return Forbid();

        // Token simple para evitar ejecuciones accidentales
        if (!string.Equals(token, "fixopolis-dev", StringComparison.Ordinal))
            return Unauthorized(new { message = "Seed token inválido." });

        await FixopolisDbContextSeed.SeedAsync(db);
        return Ok(new { message = "✅ Seed ejecutado (idempotente)." });
    }

    [HttpDelete]
    public async Task<IActionResult> Clear(
    [FromServices] FixopolisDbContext db,
    [FromServices] IWebHostEnvironment env,
    [FromHeader(Name = "X-Seed-Token")] string? token,
    CancellationToken ct)
    {
        if (!env.IsDevelopment())
            return Forbid();

        if (!string.Equals(token, "fixopolis-dev", StringComparison.Ordinal))
            return Unauthorized(new { message = "Seed token inválido." });

        // Vaciar tablas en orden por relaciones (hijo → padre)
        await db.Database.BeginTransactionAsync(ct);

        try
        {
            // Relaciones N–N y dependientes primero
            await db.ProductCategories.ExecuteDeleteAsync(ct);
            await db.OrderItems.ExecuteDeleteAsync(ct);

            // Tablas principales dependientes
            await db.Orders.ExecuteDeleteAsync(ct);
            await db.Products.ExecuteDeleteAsync(ct);
            await db.Categories.ExecuteDeleteAsync(ct);

            // Finalmente usuarios
            await db.Users.ExecuteDeleteAsync(ct);

            await db.Database.CommitTransactionAsync(ct);
            return Ok(new { message = "🧹 Base de datos vaciada correctamente." });
        }
        catch (Exception ex)
        {
            await db.Database.RollbackTransactionAsync(ct);
            return StatusCode(500, new { message = "❌ Error al vaciar la base de datos", error = ex.Message });
        }
    }

}
