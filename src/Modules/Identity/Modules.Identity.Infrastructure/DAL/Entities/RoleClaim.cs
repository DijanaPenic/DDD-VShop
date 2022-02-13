using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Identity.Infrastructure.DAL.Entities;

internal class RoleClaim : DbEntity
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; }
    public string ClaimType { get; set; }
    public string ClaimValue { get; set; }
}