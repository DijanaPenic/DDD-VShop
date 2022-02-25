using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Commands;
using VShop.Modules.Identity.Infrastructure.Attributes;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.Models;
using VShop.Modules.Identity.Infrastructure.Queries;

namespace VShop.Modules.Identity.API.Controllers;

[ApiController]
[Route("api/account")]
internal partial class AccountController : ApplicationControllerBase
{
    private const string Policy = "auth";

    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IIdentityContext _identityContext;

    public AccountController
    (
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        IContext context
    )
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _identityContext = context.Identity;
    }

    [HttpPost]
    [Route("sign-up")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ClientAuthorization]
    public async Task<IActionResult> SignUpAsync([FromBody] SignUpCommand command)
    {
        Result result = await _commandDispatcher.SendAsync(command);
        return HandleResult(result, NoContent);
    }
    
    [HttpPost]
    [Route("sign-in")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(SignInInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ClientAuthorization]
    public async Task<IActionResult> SignInAsync([FromBody] SignInByPasswordCommand command)
    {
        Result<SignInInfo> result = await _commandDispatcher.SendAsync(command);
        return HandleResult(result, Ok);
    }

    [HttpDelete]
    [Route("sign-out")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [Authorize]
    public async Task<IActionResult> SignOutAsync()
    {
        Result result = await _commandDispatcher.SendAsync(new SignOutCommand());
        return HandleResult(result, NoContent);
    }
    
    [HttpPut]
    [Route("{userId:guid}/verify")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [Authorize(AuthenticationSchemes = "ClientAuthenticationScheme, Bearer")]
    public async Task<IActionResult> VerifyAsync([FromRoute] Guid userId, [FromBody] VerifyCommand command)
    {
        Result result = await _commandDispatcher.SendAsync(command with{ UserId = userId });
        return HandleResult(result, NoContent);
    }
    
    [HttpGet]
    [Route("{userId:guid}")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [Authorize(Policy)]
    public async Task<IActionResult> GetUserAsync([FromRoute] Guid userId)
    {
        GetUserQuery query = new(userId);
        Result<User> result = await _queryDispatcher.QueryAsync(query);
        
        return HandleResult(result, Ok);
    }
    
    [HttpPost]
    [Route("{userId:guid}/send-token")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [Authorize(AuthenticationSchemes = "Identity.AccountVerification, Bearer")]
    public async Task<IActionResult> SendVerificationTokenAsync
    (
        [FromRoute] Guid userId,
        [FromBody] SendVerificationTokenCommand command
    )
    {
        bool hasPermissions = _identityContext.IsCurrentUser(userId) || _identityContext.IsAuthorized(Policy);
        if (!hasPermissions) return Forbid();

        Result result = await _commandDispatcher.SendAsync(command with { UserId = userId });
        return HandleResult(result, NoContent);
    }
    
    [HttpPut]
    [Route("{userId:guid}/set-password")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [Authorize]
    public async Task<IActionResult> SetPasswordAsync
    (
        [FromRoute] Guid userId,
        [FromBody] SetPasswordCommand command
    )
    {
        bool hasPermissions = _identityContext.IsCurrentUser(userId) || _identityContext.IsAuthorized(Policy);
        if (!hasPermissions) return Forbid();

        Result result = await _commandDispatcher.SendAsync(command with { UserId = userId });
        return HandleResult(result, NoContent);
    }
}