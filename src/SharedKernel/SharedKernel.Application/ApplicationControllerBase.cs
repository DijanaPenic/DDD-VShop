using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

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
        
        protected IActionResult HandleResult<TResult>
        (
            Result<TResult> result,
            Func<TResult, IActionResult> handleSuccess
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
                notFoundError => NotFound(notFoundError.Message),
                unauthorized => Unauthorized(unauthorized.Message)
            );
        
        protected class ChallengeResult : ActionResult
        {
            public AuthenticationProperties AuthenticationProperties { get; }
            public string AuthScheme { get; }
        
            public ChallengeResult(AuthenticationProperties authenticationProperties, string authScheme)
            {
                AuthenticationProperties = authenticationProperties;
                AuthScheme = authScheme;
            }

            public override async Task ExecuteResultAsync(ActionContext context)
            {
                if (context is null) throw new ArgumentNullException(nameof(context));

                await context.HttpContext.ChallengeAsync(AuthScheme, AuthenticationProperties);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }
    }
}