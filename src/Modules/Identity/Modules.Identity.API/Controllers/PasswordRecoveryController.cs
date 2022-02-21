using System.Net;
using Microsoft.AspNetCore.Mvc;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Commands;
using VShop.Modules.Identity.Infrastructure.Attributes;

namespace VShop.Modules.Identity.API.Controllers;

[ApiController]
[Route("api/password-recovery")]
internal class PasswordRecoveryController : ApplicationControllerBase
{
    private const string Policy = "auth";

    private readonly ICommandDispatcher _commandDispatcher;

    public PasswordRecoveryController(ICommandDispatcher commandDispatcher)
        => _commandDispatcher = commandDispatcher;

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ClientAuthorization]
    public async Task<IActionResult> InitiateAsync([FromBody] InitiatePasswordRecoveryCommand command)
    {
        Result result = await _commandDispatcher.SendAsync(command);
        return HandleResult(result, NoContent);
    }
    
    [HttpPut]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ClientAuthorization]
    public async Task<IActionResult> ResetAsync([FromBody] ResetPasswordCommand command)
    {
        Result result = await _commandDispatcher.SendAsync(command);
        return HandleResult(result, NoContent);
    }
}