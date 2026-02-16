using MediatR;
using neuranx.Application.Auth.Commands;
using neuranx.Application.Interfaces;

namespace neuranx.Application.Auth.Handlers.Query
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Domain.AuthResult>
    {
        private readonly IIdentityService _identityService;

        public LoginUserCommandHandler(IIdentityService identityService) { _identityService = identityService; }

        public async Task<Domain.AuthResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.LoginUserAsync(request.Email, request.Password);
        }
    }
}
