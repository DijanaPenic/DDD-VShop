using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Identity.Infrastructure.DAL.Entities;

internal class Client : DbEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Secret { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
    public int AccessTokenLifeTime { get; set; }
    public int RefreshTokenLifeTime { get; set; }
    public string AllowedOrigin { get; set; }
    public ICollection<UserRefreshToken> RefreshTokens { get; set; }
}