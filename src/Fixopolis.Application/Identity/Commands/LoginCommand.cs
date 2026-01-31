using MediatR;
using Fixopolis.Application.Identity.Dtos;

namespace Fixopolis.Application.Identity.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;
