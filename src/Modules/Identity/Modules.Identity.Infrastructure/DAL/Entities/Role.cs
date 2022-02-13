using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Identity.Infrastructure.DAL.Entities;

internal class Role : DbEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public bool Stackable { get; set; }
    public ICollection<RoleClaim> Claims { get; set; }
    public ICollection<User> Users { get; set; }
}