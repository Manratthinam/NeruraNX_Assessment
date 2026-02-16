using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using neuranx.Domain;

namespace neuranx.Api.Filters
{
    public class ResponseFilter : IAsyncActionFilter
    {
        private DateTime timeTaken;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            this.timeTaken = DateTime.Now;
            ActionExecutedContext executedContext = await next();

            if (executedContext.Result is ObjectResult objectResult)
            {
                var response = new ResponseDto<object>
                {
                    Code = (int)objectResult.StatusCode,
                    Payload = objectResult.Value,
                    Message = objectResult.StatusCode >= 200 && objectResult.StatusCode < 300 ? "Success" : "Error",
                    Success = objectResult.StatusCode >= 200 && objectResult.StatusCode < 300,
                    ResponseTime = (int)(DateTime.Now - timeTaken).TotalMilliseconds
                };

                context.Result = new ObjectResult(response)
                {
                    StatusCode = objectResult.StatusCode
                };
            }
        }
    }

}
