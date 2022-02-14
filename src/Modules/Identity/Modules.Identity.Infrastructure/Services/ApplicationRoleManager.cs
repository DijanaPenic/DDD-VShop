using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.Services;

internal sealed class ApplicationRoleManager : RoleManager<Role>
{
    public ApplicationRoleManager
    (
        IRoleStore<Role> roleStore,
        IEnumerable<IRoleValidator<Role>> roleValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        ILogger<ApplicationRoleManager> logger
    ) : base(roleStore, roleValidators, keyNormalizer, errors, logger) { }
}