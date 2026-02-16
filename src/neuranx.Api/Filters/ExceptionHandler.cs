using Microsoft.AspNetCore.Diagnostics;
using neuranx.Domain;

namespace neuranx.Api.Filters
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ExceptionHandler> _logger;

        public ExceptionHandler(ILogger<ExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);
            
            var response = new ResponseDto<object>
            {
                Success = false,
                ResponseTime = 0,
                Message = exception.Message,
                Code = 409
            };


            httpContext.Response.StatusCode = response.Code;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }
    }
}
