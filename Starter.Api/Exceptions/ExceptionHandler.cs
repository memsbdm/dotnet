using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Starter.Domain.Exceptions;

namespace Starter.Api.Exceptions;

public sealed class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false;
        }

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "Validation error",
            Detail = domainException.Message
        };

        httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}
