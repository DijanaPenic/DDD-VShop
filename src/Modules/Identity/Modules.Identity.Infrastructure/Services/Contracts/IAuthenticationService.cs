using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.Services.Contracts;

internal interface IAuthenticationService
{
    Task<Result<SignInResponse>> ProcessAuthAsync(SignInResult signInResult, User user);
}