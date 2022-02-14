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
            {"d92ef5e5-f08a-4173-b83a-74618893891b", new[] {"catalog", "billing", "sales"}}, // admin
            {"d92ef5e5-f08a-4173-b83a-74618893891b", new[] {"catalog"}}                      // store manager
        };

        RoleClaim[] claims = permissions.SelectMany(permission =>
        {
            (string roleId, string[] modules) = permission;
            return modules.Select(module => new RoleClaim
            {
                Id = Guid.Parse(roleId),
                RoleId = DeterministicGuid.Create(Guid.Parse(roleId), module),
                ClaimType = "permission",
                ClaimValue = module
            });
        }).ToArray();

        builder.HasData(claims);
    }
}