using NodaTime;

using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Identity.Infrastructure.DAL.Entities;

internal class UserRefreshToken : DbEntity
{
    public string Value { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid ClientId { get; set; }
    public Client Client { get; set; }
    public Instant DateExpires { get; set; }
}

public class UserRefreshTokenKey
{
    public Guid UserId { get; set; }
    public Guid ClientId { get; set; }
    public object[] ToArray() => new object[] { UserId, ClientId };
}