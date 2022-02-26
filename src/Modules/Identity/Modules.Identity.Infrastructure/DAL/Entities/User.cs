using NodaTime;

using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Identity.Infrastructure.DAL.Entities;

internal class User : DbEntity
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string NormalizedUserName { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string NormalizedEmail { get; set; }
    public string PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public string PasswordHash { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool LockoutEnabled { get; set; }
    public bool IsApproved { get; set; }
    public int AccessFailedCount { get; set; }
    public string SecurityStamp { get; set; }
    public Instant? LockoutEndDate { get; set; }
    public ICollection<Role> Roles { get; set; }
    public ICollection<UserClaim> Claims { get; set; }
    public ICollection<UserLogin> Logins { get; set; }
    public ICollection<UserToken> UserTokens { get; set; }
    public ICollection<UserRefreshToken> RefreshTokens { get; set; }
}