using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure;
using VShop.Modules.Identity.Infrastructure.Models;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.Services.Contracts;

internal interface IAuthenticationService
{
    Task<Result<SignInInfo>> FinalizeSignInAsync(SignInResult signInResult, User user);
    Task FinalizeSignOutAsync();
}