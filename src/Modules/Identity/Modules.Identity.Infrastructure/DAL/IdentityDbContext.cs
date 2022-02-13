using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.PostgresDb.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

[assembly: InternalsVisibleTo("Database.DatabaseMigrator")]
namespace VShop.Modules.Identity.Infrastructure.DAL;

internal class IdentityDbContext : DbContextBase
{
    public const string IdentitySchema = "identity";

    public DbSet<Client> Clients { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RoleClaim> RoleClaims { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserClaim> UserClaims { get; set; }
    public DbSet<UserLogin> UserLogins { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    public DbSet<UserRole> UserRoles => Set<UserRole>("user_role"); 
    
    public IdentityDbContext(IClockService clockService, IDbContextBuilder contextBuilder) 
        : base(clockService, contextBuilder) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => ContextBuilder.ConfigureContext(optionsBuilder);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.SharedTypeEntity<UserRole>("user_role", b 
            => b.HasKey(ur => new { ur.RoleId, ur.UserId }));

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}