using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Identity.Infrastructure.DAL.Entities;

internal class UserLogin : DbEntity
{
    public string LoginProvider { get; set; }
    public string ProviderKey { get; set; }
    public string ProviderDisplayName { get; set; }
    public string Token { get; set; }
    public bool IsConfirmed { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}

public class UserLoginKey
{
    public string LoginProvider { get; set; }
    public string ProviderKey { get; set; }
}