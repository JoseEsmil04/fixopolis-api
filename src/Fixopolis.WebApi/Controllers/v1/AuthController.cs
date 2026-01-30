using System.Security.Claims;
using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Identity.Commands;
using Fixopolis.Application.Identity.Commands.Login;
using Fixopolis.Application.Identity.Dtos;
using Fixopolis.Application.Identity.Queries.Me;
using Fixopolis.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixopolis.WebApi.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> Signup([FromBody] SignupCommand cmd, CancellationToken ct)
    {
        try
        {
            return Ok(await _mediator.Send(cmd, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during signup.", detail = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand cmd, CancellationToken ct)
    {
        try
        {
            return Ok(await _mediator.Send(cmd, ct));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Email o Password son incorrectos." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during login.", detail = ex.Message });
        }
    }



    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        try
        {
            var sub = User.FindFirst("sub")?.Value ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(sub, out var userId)) return Unauthorized();

            var dto = await _mediator.Send(new MeQuery(userId), ct);
            return dto is null ? NotFound() : Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving token info.", detail = ex.Message });
        }
    }

    [HttpPost("checkstatus")]
    [Authorize]
    public async Task<IActionResult> CheckStatus([FromBody] UserDto user, CancellationToken ct)
    {
        try
        {
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || userIdClaim != user.Id.ToString())
                return Unauthorized(new { message = "Invalid token or user mismatch." });

            var meQuery = new MeQuery(Guid.Parse(userIdClaim));
            var currentUser = await _mediator.Send(meQuery, ct);

            if (currentUser == null || !currentUser.IsActive)
                return Unauthorized(new { message = "User not found or inactive." });

            // Generate new token
            var userEntity = new User
            {
                Id = currentUser.Id,
                Name = currentUser.Name,
                Email = currentUser.Email,
                Role = currentUser.Role,
                IsActive = currentUser.IsActive
            };

            var tokenService = HttpContext.RequestServices.GetService<ITokenService>();
            var token = tokenService?.GenerateAccessToken(userEntity);

            if (string.IsNullOrEmpty(token))
                return StatusCode(500, new { message = "Failed to generate token." });

            return Ok(new { token, user = currentUser });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during status check.", detail = ex.Message });
        }
    }

    // Logout: stateless → el cliente debe borrar el token.
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout() => Ok(new { message = "Logged out. Client must discard tokens." });
}
