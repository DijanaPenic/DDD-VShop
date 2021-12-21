using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.SharedKernel.Application
{
    public class ApplicationControllerBase : ControllerBase
    {
        protected IActionResult InternalServerError(string message) 
            => StatusCode(StatusCodes.Status500InternalServerError, message);
        
        protected IActionResult Created(object value) 
            => StatusCode(StatusCodes.Status201Created, value);
        
        protected IActionResult Created() 
            => StatusCode(StatusCodes.Status201Created);
        
        protected IActionResult HandleResult<TData>
        (
            Result<TData> result,
            Func<TData, IActionResult> handleSuccess
        ) => result.Match
        (
            successResult => handleSuccess(successResult.Value),
            HandleError
        );

        protected IActionResult HandleResult
        (
            Result result,
            Func<IActionResult> handleSuccess
        ) => result.Match
        (
            successResult => handleSuccess(),
            HandleError
        );

        protected IActionResult HandleError(ApplicationError error)
            => error.Match
            (
                validationError => BadRequest(validationError.Message),
                systemError => InternalServerError(systemError.Message),
                notFoundError => NotFound(notFoundError.Message)
            );
    }
}