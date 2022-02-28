using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Models;
using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.Services.Contracts;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.Commands.Handlers
{
    internal class SignInByPasswordCommandHandler : ICommandHandler<SignInByPasswordCommand, SignInInfo>
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IAuthenticationService _authService;
        private readonly IIdentityContext _identityContext;

        public SignInByPasswordCommandHandler
        (
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            IAuthenticationService authService,
            IContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
            _identityContext = context.Identity;
        }

        public async Task<Result<SignInInfo>> HandleAsync
        (
            SignInByPasswordCommand command,
            CancellationToken cancellationToken
        )
        {
            (string userName, string password) = command;
            
            User user = await _userManager.FindByNameAsync(userName);
            
            if (user is null) return Result.Unauthorized("Failed to log in - invalid username and/or password.");
            if (!user.IsApproved) return Result.Unauthorized($"User [{userName}] is not approved.");
            
            SignInResult signInResult = await _signInManager.PasswordSignInAsync
            (
                _identityContext.ClientId,
                user,
                password,
                true
            );

            return await _authService.FinalizeSignInAsync(signInResult, user);
        }
    }
}