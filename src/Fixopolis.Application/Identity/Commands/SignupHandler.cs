using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Identity.Dtos;
using Fixopolis.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Identity.Commands.Signup;

public sealed class SignupHandler : IRequestHandler<SignupCommand, UserDto>
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _hasher;

    public SignupHandler(IAppDbContext db, IPasswordHasher hasher)
    {
        _db = db; _hasher = hasher;
    }

    public async Task<UserDto> Handle(SignupCommand req, CancellationToken ct)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == req.Email, ct);
        if (exists) throw new InvalidOperationException("El Email utilizado ya esta registrado!.");

        var user = new User
        {
            Name = req.Name,
            Email = req.Email,
            PasswordHash = _hasher.Hash(req.Password),
            Role = req.Role,
            IsActive = true
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return new UserDto(user.Id, user.Name, user.Email, user.Role, user.IsActive);
    }
}
