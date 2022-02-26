using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.DAL.Configurations;

internal class UserRefreshTokenEntityTypeConfiguration : IEntityTypeConfiguration<UserRefreshToken> 
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        // Maps to the RefreshToken table
        builder.ToTable("user_refresh_token", IdentityDbContext.IdentitySchema);

        // Primary key
        builder.HasKey(urt => new { urt.UserId, urt.ClientId });

        // Limit the size of database columns
        builder.Property(urt => urt.Value).IsRequired().HasMaxLength(256);
        builder.Property(urt => urt.DateExpires).IsRequired();

        // The relationship between User and UserRefreshToken
        builder.HasOne(urt => urt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(urt => urt.UserId)
            .IsRequired();

        // The relationship between Client and UserRefreshToken
        builder.HasOne(urt => urt.Client)
            .WithMany(c => c.RefreshTokens)
            .HasForeignKey(urt => urt.ClientId)
            .IsRequired();
        
        // A concurrency token for use with the optimistic concurrency checking
        builder.UseXminAsConcurrencyToken();
    }
}