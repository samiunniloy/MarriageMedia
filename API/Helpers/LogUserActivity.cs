using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IActionFilter
    {
        public async Task OnActionExecuted(ActionExecutedContext context,ActionExecutionDelegate next)
        {
            var resultContext = await next();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
