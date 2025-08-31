using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MotoNow.Application.Results;

namespace MotoNow.API.Filters;

public sealed class ApiResponseEnvelopeFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executed = await next();

        if (executed.Exception != null || executed.Canceled) return;

        if (executed.Result is ObjectResult obj)
        {
            var status = obj.StatusCode ?? StatusCodes.Status200OK;

            if (obj.Value is IApiEnvelope) return;

            if (status is >= 200 and < 300
                && obj.Value is not ProblemDetails
                && executed.Result is not FileResult)
            {
                executed.Result = new ObjectResult(new ApiSuccess<object?>(true, "", obj.Value))
                {
                    StatusCode = status
                };
            }
            return;
        }

        if (executed.Result is OkResult)
            executed.Result = new ObjectResult(new ApiSuccess<object?>(true, "", null))
            { StatusCode = StatusCodes.Status200OK };

    }
}
