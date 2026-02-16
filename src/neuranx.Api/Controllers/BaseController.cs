using MediatR;
using Microsoft.AspNetCore.Mvc;
using neuranx.Application.Interfaces;

namespace neuranx.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<T> : ControllerBase where T : BaseController<T>
    {
        private IMediator? mediator;
        private ICurrentUserService? currentUserService;
        private ILogger<T>? logger;
        protected IMediator Mediator => mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        protected ICurrentUserService CurrentUserService => currentUserService ??= HttpContext.RequestServices.GetService<ICurrentUserService>();
        protected ILogger<T> Logger => logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
    }
}
