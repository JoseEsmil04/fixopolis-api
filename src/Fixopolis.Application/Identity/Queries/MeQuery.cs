using Fixopolis.Application.Identity.Dtos;
using MediatR;

namespace Fixopolis.Application.Identity.Queries.Me;

public sealed record MeQuery(Guid UserId) : IRequest<UserDto?>;
