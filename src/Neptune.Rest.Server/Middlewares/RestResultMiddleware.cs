using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Neptune.Server.Core.Data.Rest.Base;

namespace Neptune.Rest.Server.Middlewares;

/// <summary>
/// Middleware that automatically converts RestResultObject to appropriate IActionResult
/// </summary>
public class RestResultMiddleware : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Execute the action
        var executedContext = await next();

        // Check if the result is already an IActionResult
        if (executedContext.Result is ObjectResult objectResult)
        {
            // Check if the value is a RestResultObject
            if (objectResult.Value != null && IsRestResultObject(objectResult.Value.GetType()))
            {
                // Get the value using reflection
                dynamic restResult = objectResult.Value;
                int statusCode = (int)restResult.StatusCode;

                // Create a new result with the appropriate status code
                executedContext.Result = new ObjectResult(restResult)
                {
                    StatusCode = statusCode
                };
            }
        }
    }

    private bool IsRestResultObject(Type type)
    {
        if (type == null)
        {
            return false;
        }

        // Check if it's a RestResultObject<T>
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(RestResultObject<>))
        {
            return true;
        }

        return false;
    }
}
