using Microsoft.AspNetCore.Identity;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.DAL.Stores.Contracts;

internal interface IApplicationUserStore : IUserStore<User>
{
    Task ApproveUserAsync(User user, CancellationToken cancellationToken);
    Task DisapproveUserAsync(User user, CancellationToken cancellationToken);
}