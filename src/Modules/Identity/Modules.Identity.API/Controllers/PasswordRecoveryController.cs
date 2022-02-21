using System.Net;
using Microsoft.AspNetCore.Mvc;

using VShop.SharedKernel.Infrastructure;
using VShop.Modules.Identity.Infrastructure.Commands;
using VShop.Modules.Identity.Infrastructure.Attributes;

namespace VShop.Modules.Identity.API.Controllers;

internal partial class AccountController
{
    [HttpPost]
    [Route("password-recovery")]
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
    [Route("password-recovery")]
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