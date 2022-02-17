using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Commands;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.Attributes;

namespace VShop.Modules.Identity.API.Controllers;

[ApiController]
[Route("api/account")]
internal class AccountController : ApplicationControllerBase
{
    private const string Policy = "auth";

    private readonly ICommandDispatcher _commandDispatcher;

    public AccountController(ICommandDispatcher commandDispatcher) => _commandDispatcher = commandDispatcher;

    [HttpPost]
    [Route("sign-up")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ClientAuthorization]
    public async Task<IActionResult> SignUpAsync([FromBody] SignUpCommand request)
    {
        Result result = await _commandDispatcher.SendAsync(request);
        return HandleResult(result, NoContent);
    }
    
    [HttpPost]
    [Route("sign-in")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ClientAuthorization]
    public async Task<IActionResult> SignInAsync([FromBody] SignInCommand request)
    {
        Result<SignInResponse> result = await _commandDispatcher.SendAsync(request);
        return HandleResult(result, Ok);
    }
    
    // For testing purposes.
    [HttpPost]
    [Route("test-auth")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [Authorize(Policy)]
    public IActionResult TestUpAsync()
    {
        return Ok();
    }
}