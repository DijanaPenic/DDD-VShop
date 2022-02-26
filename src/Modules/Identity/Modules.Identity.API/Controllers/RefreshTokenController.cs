using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using VShop.SharedKernel.Infrastructure;
using VShop.Modules.Identity.Infrastructure.Commands;

namespace VShop.Modules.Identity.API.Controllers;

internal partial class AccountController
{
    [HttpDelete]
    [Route("refresh-tokens/expired")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [Authorize(Policy)]
    public async Task<IActionResult> DeleteAsync()
    {
        Result result = await _commandDispatcher.SendAsync(new DeleteExpiredRefreshTokensCommand());
        return HandleResult(result, NoContent);
    }
}