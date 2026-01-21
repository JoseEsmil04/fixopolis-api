using Fixopolis.Application.Abstractions;
using Fixopolis.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class SeedController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Run(
        [FromServices] FixopolisDbContext db,
        [FromServices] IWebHostEnvironment env,
        [FromServices] IPasswordHasher hasher,
        [FromHeader(Name = "X-Seed-Token")] string? token,
        CancellationToken ct)
    {
        // Temporarily allow seed in all environments for testing
        // if (!env.IsDevelopment())
        //     return Forbid();

        if (!string.Equals(token, "fixopolis-dev", StringComparison.Ordinal))
            return Unauthorized(new { message = "Seed token inválido." });

        var contentRoot = env.ContentRootPath;
        await FixopolisSeed.SeedAsync(db, contentRoot, hasher, ct);
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

        await db.Database.BeginTransactionAsync(ct);
        try
        {
            await db.OrderItems.ExecuteDeleteAsync(ct);
            await db.Orders.ExecuteDeleteAsync(ct);
            await db.Products.ExecuteDeleteAsync(ct);
            await db.Categories.ExecuteDeleteAsync(ct);
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
