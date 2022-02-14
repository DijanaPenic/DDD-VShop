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

internal class UserTokenKey
{
    public Guid UserId { get; set; }
    public string LoginProvider { get; set; }
    public string Name { get; set; }
    public object[] ToArray() => new object[] { UserId, LoginProvider, Name };
}