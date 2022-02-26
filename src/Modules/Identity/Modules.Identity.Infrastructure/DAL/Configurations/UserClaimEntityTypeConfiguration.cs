using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.DAL.Configurations;

internal class UserClaimEntityTypeConfiguration : IEntityTypeConfiguration<UserClaim> 
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        // Maps to the UserClaim table
        builder.ToTable("user_claim", IdentityDbContext.IdentitySchema);

        // Primary key
        builder.HasKey(uc => uc.Id);
        
        // Requirements
        builder.Property(uc => uc.ClaimType).IsRequired();
        builder.Property(uc => uc.ClaimValue).IsRequired();

        // The relationship between User and UserClaim
        builder.HasOne(uc => uc.User)
            .WithMany(u => u.Claims)
            .HasForeignKey(uc => uc.UserId)
            .IsRequired();
        
        // A concurrency token for use with the optimistic concurrency checking
        builder.UseXminAsConcurrencyToken();
    }
}