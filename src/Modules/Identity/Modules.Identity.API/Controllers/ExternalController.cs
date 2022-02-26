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

    [HttpGet]
    [Route("external/callback")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ClientAuthorization]
    public async Task<IActionResult> GetCallbackAsync([FromQuery] string provider, [FromQuery] string returnUrl)
    {
        InitiateExternalLoginCommand command = new(provider, returnUrl);
        Result<AuthenticationProperties> result = await _commandDispatcher.SendAsync(command);
        
        // Location's return_url (in the challenge response) will point back to API for external authentication
        // (by default: the "signin-google" endpoint). After successful authentication, the user will be redirected to
        // the return_url from this endpoint (the client app).
        return new ChallengeResult(result.Data, command.Provider);
    }
    
    [HttpPost]
    [Route("external/sign-in")]
    [ProducesResponseType(typeof(SignInInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [Authorize(AuthenticationSchemes = "Identity.External")]
    public async Task<IActionResult> SignInAsync([FromQuery] string confirmationUrl)
    {
        SignInExternalCommand command = new(confirmationUrl);
        Result<SignInInfo> result = await _commandDispatcher.SendAsync(command);
        
        return HandleResult(result, Ok);
    }

    [HttpPost]
    [Route("external/sign-up")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [Authorize(AuthenticationSchemes = "Identity.External")]
    public async Task<IActionResult> SignUpAsync([FromBody] SignUpExternalCommand command)
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
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [Authorize]
    public async Task<IActionResult> DeleteExternalLoginAsync
    (
        [FromRoute] Guid userId,
        [FromQuery] string loginProvider,
        [FromQuery] string providerKey
    )
    {
        bool hasPermissions = _identityContext.IsCurrentUser(userId) || _identityContext.IsAuthorized(Policy);
        if (!hasPermissions) return Forbid();

        DisconnectExternalLoginCommand command = new(userId, loginProvider, providerKey);
        Result result = await _commandDispatcher.SendAsync(command);
        
        return HandleResult(result, NoContent);
    }
    
    [HttpPut]
    [Route("{userId:guid}/external/verify")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
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
        VerifyExternalCommand command = new(userId, token);
        Result result = await _commandDispatcher.SendAsync(command);
        
        return HandleResult(result, NoContent);
    }
}