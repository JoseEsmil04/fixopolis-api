using Fixopolis.Application.Identity.Dtos;
using Fixopolis.Domain.Utils;
using MediatR;

namespace Fixopolis.Application.Identity.Commands;

public sealed record SignupCommand(string Name, string Email, string Password, UserRole Role = UserRole.Employee)
    : IRequest<UserDto>;