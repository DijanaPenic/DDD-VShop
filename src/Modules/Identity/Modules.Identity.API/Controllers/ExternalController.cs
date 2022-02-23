using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

using VShop.SharedKernel.Infrastructure;
using VShop.Modules.Identity.Infrastructure.Models;
using VShop.Modules.Identity.Infrastructure.Commands;
using VShop.Modules.Identity.Infrastructure.Queries;
using VShop.Modules.Identity.Infrastructure.Attributes;

namespace VShop.Modules.Identity.API.Controllers;

internal partial class AccountController
{
    [HttpGet]
    [Route("external/providers")]
    [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ClientAuthorization]
    public async Task<IActionResult> GetProvidersAsync()
    {
        Result<List<string>> result = await _queryDispatcher.QueryAsync(new GetExternalProvidersQuery());
        
        if (result.IsError) return HandleError(result.Error);
        if (result.Data.Count is 0) return NoContent();
        
        return Ok(result.Data);
    }
    
    [HttpPost]
    [Route("external/initiate")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ClientAuthorization]
    public async Task<IActionResult> InitiateAsync([FromBody] InitiateExternalLoginCommand command)
    {
        Result<AuthenticationProperties> result = await _commandDispatcher.SendAsync(command);
        return new ChallengeResult(result.Data, command.Provider);
    }

    [HttpPost]
    [Route("external/sign-up")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [Authorize(AuthenticationSchemes = "Identity.External")] // TODO - cannot combine with the ClientId verification.
    public async Task<IActionResult> SignUpAsync([FromBody] SignUpExternalCommand command)
    {
        Result result = await _commandDispatcher.SendAsync(command);
        return HandleResult(result, NoContent);
    }
    
    [HttpPost]
    [Route("external/sign-in")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [Authorize(AuthenticationSchemes = "Identity.External")]
    public async Task<IActionResult> SignInAsync([FromBody] SignInExternalCommand command)
    {
        Result result = await _commandDispatcher.SendAsync(command);
        return HandleResult(result, NoContent);
    }
    
    [HttpGet]
    [Route("{userId:guid}/external")]
    [ProducesResponseType(typeof(List<ExternalLoginInfo>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [Authorize]
    public async Task<IActionResult> GetExternalLoginsAsync([FromRoute] Guid userId)
    {
        bool hasPermissions = _identityContext.IsCurrentUser(userId) || _identityContext.IsAuthorized(Policy);
        if (!hasPermissions) return Forbid();

        GetExternalLoginQuery query = new(userId);
        Result<List<ExternalLoginInfo>> result = await _queryDispatcher.QueryAsync(query);

        if (result.IsError) return HandleError(result.Error);
        if (result.Data.Count is 0) return NoContent();
        
        return Ok(result.Data);
    }
    
    [HttpDelete]
    [Route("{userId:guid}/external")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [Authorize]
    public async Task<IActionResult> DeleteExternalLoginAsync
    (
        [FromRoute] Guid userId,
        [FromBody] DisconnectExternalLoginCommand command
    )
    {
        bool hasPermissions = _identityContext.IsCurrentUser(userId) || _identityContext.IsAuthorized(Policy);
        if (!hasPermissions) return Forbid();
        
        Result result = await _commandDispatcher.SendAsync(command with { UserId = userId });
        return HandleResult(result, NoContent);
    }
    
    [HttpPut]
    [Route("{userId:guid}/external/verify")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ClientAuthorization]
    public async Task<IActionResult> ConfirmExternalLoginAsync
    (
        [FromRoute] Guid userId,
        [FromQuery] string token
    )
    {
        VerifyExternalCommand command = new(userId, token); // TODO - validation.
        Result result = await _commandDispatcher.SendAsync(command);
        
        return HandleResult(result, NoContent);
    }
}