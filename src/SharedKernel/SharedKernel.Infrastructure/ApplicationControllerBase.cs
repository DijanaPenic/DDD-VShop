using OneOf;
using OneOf.Types;
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
        
        protected IActionResult Created() 
            => StatusCode(StatusCodes.Status201Created);

        protected IActionResult HandleObjectResult<TResult>
        (
            OneOf<Success<TResult>, ApplicationError> result,
            Func<TResult, IActionResult> handleSuccess
        )
        {
            return result.Match
            (
                data => handleSuccess(data.Value),
                HandleError
            );
        }
        
        protected IActionResult HandleResult<TResult>
        (
            OneOf<TResult, ApplicationError> result,
            Func<IActionResult> handleSuccess
        )
        {
            return result.Match
            (
                _ => handleSuccess(),
                HandleError
            );
        }

        private IActionResult HandleError(ApplicationError error)
        {
            return error.Match
            (
                validationError => BadRequest(validationError.Message),
                systemError => InternalServerError(systemError.Message),
                notFoundError => NotFound(notFoundError.Message)
            );
        }
    }
}