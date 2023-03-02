using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Topluluk.Shared.Helper
{
    public class ValidationHelper : IAsyncActionFilter
    {
        // Actiona gelen isteklerde devreye giren bir filter
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Where(x => x.Value.Errors.Any())
                    .ToDictionary(e => e.Key, e => e.Value.Errors.Select(e => e.ErrorMessage))
                    .ToArray();
                context.Result = new BadRequestObjectResult(errors);
                return;
            }

            await next();
        }
    }
}

