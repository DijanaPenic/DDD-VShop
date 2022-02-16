using System.Net;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using VShop.Modules.Identity.Infrastructure.Commands;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure.Auth;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace VShop.Modules.Identity.API.Controllers;

[ApiController]
[Route("api/account")]
internal class AccountController : ApplicationControllerBase
{
    private const string Policy = "auth";

    private readonly ApplicationUserManager _userManager;
    private readonly ApplicationSignInManager _signInManager;
    private readonly ApplicationAuthManager _authManager;
    private readonly ICommandDispatcher _commandDispatcher;
    
    public AccountController
    (
        ApplicationUserManager userManager,
        ApplicationSignInManager signInManager,
        ApplicationAuthManager authManager,
        ICommandDispatcher commandDispatcher
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authManager = authManager;
        _commandDispatcher = commandDispatcher;
    }
    
    // TODO - missing client auth.
    [HttpPost]
    [Route("sign-up")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> SignUpAsync([FromBody] SignUpCommand request)
    {
        // User user = new()
        // {
        //     Id = SequentialGuid.Create(),
        //     Email = "penic.dijana@gmail.com",
        //     UserName = "dpenic",
        //     IsApproved = true
        // };
        
        //IdentityResult userResult = await _userManager.CreateAsync(user, request.Password);
        User user = await _userManager.FindByEmailAsync("PENIC.DIJANA@GMAIL.COM");
        
        //IdentityResult roleResult = await _userManager.AddToRoleAsync(user, "ADMIN");
        
        SignInResult registerResult = await _signInManager.RegisterAsync(Guid.Parse("5c52160a-4ab4-49c6-ba5f-56df9c5730b6"), user);

        return Ok();
    }
    
    [HttpPost]
    [Route("sign-in")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> SignUpAsync([FromBody] SignInCommand request)
    {
        User user = await _userManager.FindByNameAsync(request.UserName);
        Guid clientId = Guid.Parse("5c52160a-4ab4-49c6-ba5f-56df9c5730b6");
        
        SignInResult signInResult = await _signInManager.PasswordSignInAsync(clientId, user,
            request.Password, lockoutOnFailure: true);

        JsonWebToken token = await _authManager.CreateTokenAsync(user.Id, clientId);

        await _signInManager.SignInWithJsonWebTokenAsync(user.Id.ToString(), token.AccessToken);

        return Ok();
    }
    
    [Authorize(Policy)]
    [HttpPost]
    [Route("test-auth")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult TestUpAsync()
    {
        return Ok();
    }
}