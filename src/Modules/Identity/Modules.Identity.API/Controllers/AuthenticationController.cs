using System.Net;
using Microsoft.AspNetCore.Mvc;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Commands;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.Attributes;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;

namespace VShop.Modules.Identity.API.Controllers;

[ApiController]
[Route("api/auth")]
internal class AuthenticationController : ApplicationControllerBase
{
    private const string Policy = "auth";

    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IIdentityContext _identityContext;

    public AuthenticationController(ICommandDispatcher commandDispatcher, IContext context)
    {
        _commandDispatcher = commandDispatcher;
        _identityContext = context.Identity;
    }

    [HttpPost]
    [Route("sign-in")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ClientAuthorization]
    public async Task<IActionResult> SignInAsync([FromBody] SignInCommand command)
    {
        Result<SignInResponse> result = await _commandDispatcher.SendAsync(command);
        return HandleResult(result, Ok);
    }
}