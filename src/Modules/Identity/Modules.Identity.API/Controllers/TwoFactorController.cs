using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using VShop.SharedKernel.Infrastructure;
using VShop.Modules.Identity.Infrastructure.Models;
using VShop.Modules.Identity.Infrastructure.Queries;
using VShop.Modules.Identity.Infrastructure.Commands;

namespace VShop.Modules.Identity.API.Controllers;

internal partial class AccountController
{
    [HttpPost]
    [Route("two-factor/sign-in")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(SignInInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [Authorize(AuthenticationSchemes = "Identity.TwoFactorUserId")]
    public async Task<IActionResult> SignInAsync([FromBody] SignInByTwoFactorCommand command)
    {
        Result<SignInInfo> result = await _commandDispatcher.SendAsync(command);
        return HandleResult(result, Ok);
    }
    
    [HttpGet]
    [Route("{userId:guid}/two-factor/authenticator")]
    [ProducesResponseType(typeof(AuthenticatorKey), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [Authorize]
    public async Task<IActionResult> GetAuthenticatorKeyAsync([FromRoute] Guid userId)
    {
        bool hasPermissions = _identityContext.IsCurrentUser(userId) || _identityContext.IsAuthorized(Policy);
        if (!hasPermissions) return Forbid();
        
        GetAuthenticatorKeyQuery query = new(userId);
        Result<AuthenticatorKey> result = await _queryDispatcher.QueryAsync(query);
        
        return HandleResult(result, Ok);
    }
    
    [HttpPost]
    [Route("{userId:guid}/two-factor/authenticator/renew")]
    [ProducesResponseType(typeof(AuthenticatorKey), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [Authorize]
    public async Task<IActionResult> RenewAuthenticatorKeyAsync([FromRoute] Guid userId)
    {
        bool hasPermissions = _identityContext.IsCurrentUser(userId) || _identityContext.IsAuthorized(Policy);
        if (!hasPermissions) return Forbid();

        RenewAuthenticatorKeyCommand command = new(userId);
        Result<AuthenticatorKey> result = await _commandDispatcher.SendAsync(command);
        
        return HandleResult(result, Ok);
    }
    
    [HttpPost]
    [Route("{userId:guid}/two-factor/recovery-codes/renew")]
    [ProducesResponseType(typeof(RecoveryCodes), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [Authorize]
    public async Task<IActionResult> RenewRecoveryCodesAsync([FromRoute] Guid userId, [FromQuery] int number)
    {
        bool hasPermissions = _identityContext.IsCurrentUser(userId) || _identityContext.IsAuthorized(Policy);
        if (!hasPermissions) return Forbid();

        RenewRecoveryCodesCommand command = new(userId, number);
        Result<RecoveryCodes> result = await _commandDispatcher.SendAsync(command);
        
        return HandleResult(result, Ok);
    }
    
    [HttpPut]
    [Route("{userId:guid}/two-factor/enable")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [Authorize]
    public async Task<IActionResult> EnableAsync([FromRoute] Guid userId, [FromQuery] string code)
    {
        bool hasPermissions = _identityContext.IsCurrentUser(userId) || _identityContext.IsAuthorized(Policy);
        if (!hasPermissions) return Forbid();

        EnableTwoFactorCommand command = new(userId, code);
        Result result = await _commandDispatcher.SendAsync(command);
        
        return HandleResult(result, NoContent);
    }
    
    [HttpPut]
    [Route("{userId:guid}/two-factor/disable")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [Authorize]
    public async Task<IActionResult> DisableAsync([FromRoute] Guid userId)
    {
        bool hasPermissions = _identityContext.IsCurrentUser(userId) || _identityContext.IsAuthorized(Policy);
        if (!hasPermissions) return Forbid();

        DisableTwoFactorCommand command = new(userId);
        Result result = await _commandDispatcher.SendAsync(command);
        
        return HandleResult(result, NoContent);
    }
}