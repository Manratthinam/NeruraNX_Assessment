using MediatR;
using neuranx.Application.Auth.Commands;
using neuranx.Application.Interfaces;

namespace neuranx.Application.Auth.Handlers.Command
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
    {
        private readonly IIdentityService _identityService;

        public RegisterUserCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.RegisterUserAsync(request.UserName, request.Email, request.Password);
        }
    }
}
