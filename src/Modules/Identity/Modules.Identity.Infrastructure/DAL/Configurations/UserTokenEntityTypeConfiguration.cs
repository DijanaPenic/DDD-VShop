using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.DAL.Configurations;

internal class UserTokenEntityTypeConfiguration : IEntityTypeConfiguration<UserToken> 
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        // Maps to the UserToken table
        builder.ToTable("user_token", IdentityDbContext.IdentitySchema);

        // Composite primary key consisting of the UserId, LoginProvider and Name
        builder.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

        // Limit the size of database columns
        builder.Property(ut => ut.LoginProvider).IsRequired().HasMaxLength(128);
        builder.Property(ut => ut.Name).IsRequired().HasMaxLength(128);
        builder.Property(ut => ut.Value).IsRequired();

        // The relationship between User and UserToken
        builder.HasOne(ut => ut.User)
            .WithMany(u => u.UserTokens)
            .HasForeignKey(ut => ut.UserId)
            .IsRequired();
        
        // A concurrency token for use with the optimistic concurrency checking
        builder.UseXminAsConcurrencyToken();
    }
}