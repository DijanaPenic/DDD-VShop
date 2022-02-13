using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Identity.Infrastructure.DAL.Entities;

internal class UserToken : DbEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string LoginProvider { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}