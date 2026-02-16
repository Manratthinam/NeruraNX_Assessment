using MediatR;
using neuranx.Application.Auth.Commands;
using neuranx.Application.Interfaces;

namespace neuranx.Application.Auth.Handlers.Query
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, object>
    {

        private readonly IIdentityService _identityService;

        public GetAllUsersQueryHandler(IIdentityService identityService) { _identityService = identityService; }

        public async Task<object> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return await _identityService.GetAllUsersAsync();
        }
    }
}
