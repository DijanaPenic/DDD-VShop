using Microsoft.AspNetCore.Identity;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.DAL.Stores.Contracts;

internal interface IApplicationLoginUserStore : IUserLoginStore<User>
{
    Task AddLoginAsync(User user, UserLoginInfo login, string token, CancellationToken cancellationToken);
    Task UpdateLoginAsync(UserLogin login, CancellationToken cancellationToken);
    Task ConfirmLoginAsync(UserLogin login, CancellationToken cancellationToken);
    Task<UserLogin> FindLoginAsync(UserLoginInfo login, CancellationToken cancellationToken);
    Task<UserLogin> FindLoginAsync(User user, string token, CancellationToken cancellationToken);
    Task<User> FindByLoginAsync(UserLoginInfo login, bool loginConfirmed, CancellationToken cancellationToken);
}