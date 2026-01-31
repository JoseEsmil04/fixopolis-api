using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Identity.Dtos;
using Fixopolis.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Identity.Commands.Signup;

public sealed class SignupHandler : IRequestHandler<SignupCommand, AuthResponseDto>
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public SignupHandler(IAppDbContext db, IPasswordHasher hasher, ITokenService tokens)
    {
        _db = db; _hasher = hasher; _tokens = tokens;
    }

    public async Task<AuthResponseDto> Handle(SignupCommand req, CancellationToken ct)
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

        var token = _tokens.GenerateAccessToken(user);
        var userDto = new UserDto(user.Id, user.Name, user.Email, user.Role, user.IsActive);
        return new AuthResponseDto(token, userDto);
    }
}
