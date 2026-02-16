using MediatR;
using neuranx.Application.Auth.Commands;
using neuranx.Application.Interfaces;
using neuranx.Domain;

namespace neuranx.Application.Auth.Handlers.Command;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);
    }
}
