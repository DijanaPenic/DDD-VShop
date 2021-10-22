using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace VShop.SharedKernel.Infrastructure
{
    public class ApplicationControllerBase : ControllerBase
    {
        protected IActionResult InternalServerError(string message) 
            => StatusCode(StatusCodes.Status500InternalServerError, message);
        
        protected IActionResult Created(object value) 
            => StatusCode(StatusCodes.Status201Created, value);
    }
}