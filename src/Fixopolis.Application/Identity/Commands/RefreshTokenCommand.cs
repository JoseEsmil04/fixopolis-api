using MediatR;
using Fixopolis.Application.Identity.Dtos;

namespace Fixopolis.Application.Identity.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResultDto>;
