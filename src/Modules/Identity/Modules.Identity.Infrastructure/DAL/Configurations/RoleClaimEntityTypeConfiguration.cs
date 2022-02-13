using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
    }
}