using OneOf;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.SharedKernel.Infrastructure
{
    public class ApplicationControllerBase : ControllerBase
    {
        protected IActionResult InternalServerError(string message) 
            => StatusCode(StatusCodes.Status500InternalServerError, message);
        
        protected IActionResult Created(object value) 
            => StatusCode(StatusCodes.Status201Created, value);

        protected IActionResult HandleResult<TResult>(OneOf<TResult, ApplicationError> result, Func<TResult, IActionResult> success)
        {
            return result.Match
            (
                success,
                error => error.Match
                (
                    validationError => BadRequest(validationError.Message),
                    systemError => InternalServerError(systemError.Message),
                    notFoundError => NotFound(notFoundError.Message)
                )
            );
        }
    }
}