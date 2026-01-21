using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Identity.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Identity.Queries.Me;

public sealed class MeHandler : IRequestHandler<MeQuery, UserDto?>
{
    private readonly IAppDbContext _db;
    public MeHandler(IAppDbContext db) => _db = db;

    public async Task<UserDto?> Handle(MeQuery q, CancellationToken ct)
    {
        var u = await _db.Users.AsNoTracking()
            .Where(x => x.Id == q.UserId)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Email,
                x.Role,
                x.IsActive
            })
            .FirstOrDefaultAsync(ct);

        if (u is null) return null;

        return new UserDto(
            u.Id,
            u.Name ?? string.Empty,
            u.Email ?? string.Empty,
            u.Role,
            u.IsActive
        );
    }
}
