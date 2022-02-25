using Serilog;
using System.Net;
using Microsoft.AspNetCore.Http;

using AppExceptions = VShop.SharedKernel.Infrastructure.Exceptions; 

namespace VShop.SharedKernel.Application.Exceptions;

internal sealed class ErrorHandlerMiddleware : IMiddleware
{
    private readonly ILogger _logger;

    public ErrorHandlerMiddleware(ILogger logger) => _logger = logger;
        
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unhandled error has occurred");
            await HandleErrorAsync(context, ex);
        }
    }

    private async Task HandleErrorAsync(HttpContext context, Exception exception)
    {
        (object message, HttpStatusCode statusCode) = MapToResponse(exception);
        context.Response.StatusCode = (int)statusCode;

        if (message is null) return;
        await context.Response.WriteAsJsonAsync(message);
    }
    
    private ExceptionResponse MapToResponse(Exception exception)
        => exception switch
        {
            FluentValidation.ValidationException ex => new ExceptionResponse
            (
                ex.Errors.Select(e => new
                {
                    e.PropertyName,
                    e.ErrorMessage
                }),
                HttpStatusCode.BadRequest
            ),
            AppExceptions.ValidationException => new ExceptionResponse(exception.Message, HttpStatusCode.BadRequest),
            _ => new ExceptionResponse("An error has occurred.", HttpStatusCode.InternalServerError)
        };
    
    private record ExceptionResponse(object Message, HttpStatusCode StatusCode);
}