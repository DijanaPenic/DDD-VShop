using NodaTime.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.SharedKernel.Infrastructure.Types;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.DAL.Configurations;

internal class RoleClaimEntityTypeConfiguration : IEntityTypeConfiguration<RoleClaim> 
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        // Maps to the RoleClaim table
        builder.ToTable("role_claim", IdentityDbContext.IdentitySchema);

        // Primary key
        builder.HasKey(rc => rc.Id);
        
        // Requirements
        builder.Property(rc => rc.ClaimType).IsRequired();
        builder.Property(rc => rc.ClaimValue).IsRequired();

        // The relationship between Role and RoleClaim
        builder.HasOne(rc => rc.Role)
            .WithMany(r => r.Claims)
            .HasForeignKey(rc => rc.RoleId)
            .IsRequired();
        
        // A concurrency token for use with the optimistic concurrency checking
        builder.UseXminAsConcurrencyToken();

        IDictionary<string, string[]> permissions = new Dictionary<string, string[]>()
        {
            // admin
            {
                "d72ef5e5-f08a-4173-b83a-74618893891b",
                new[] {"orders", "shopping_carts", "auth", "payments", "categories", "products"}
            }, 
            
            // store manager
            {
                "d92ef5e5-f08a-4173-b83a-74618893891b",
                new[] {"categories", "products"}
            }
        };

        IEnumerable<RoleClaim> claims = permissions.SelectMany(permission =>
        {
            (string roleId, string[] policies) = permission;
            return policies.Select(policy => new RoleClaim
            {
                Id = DeterministicGuid.Create(Guid.Parse(roleId), policy),
                RoleId = Guid.Parse(roleId),
                ClaimType = "permission",
                ClaimValue = policy,
                DateCreated = InstantPattern.General.Parse("2022-01-01T00:00:00Z").Value,
                DateUpdated = InstantPattern.General.Parse("2022-01-01T00:00:00Z").Value
            });
        });

        builder.HasData(claims);
    }
}