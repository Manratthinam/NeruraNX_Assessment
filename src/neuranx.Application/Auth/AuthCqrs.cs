using MediatR;
using neuranx.Domain;

namespace neuranx.Application.Auth.Commands;

public record RegisterUserCommand(string UserName, string Email, string Password) : IRequest<bool>;
public record LoginUserCommand(string Email, string Password) : IRequest<neuranx.Domain.AuthResult>;

public record GetAllUsersQuery() : IRequest<object>;

public record RefreshTokenCommand(string Token, string RefreshToken) : IRequest<AuthResult>;

