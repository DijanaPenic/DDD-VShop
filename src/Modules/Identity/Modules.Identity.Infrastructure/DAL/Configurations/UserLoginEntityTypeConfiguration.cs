using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.DAL.Configurations;

internal class UserLoginEntityTypeConfiguration : IEntityTypeConfiguration<UserLogin> 
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        // Maps to the UserLogin table
        builder.ToTable("user_login", IdentityDbContext.IdentitySchema);

        // Composite primary key consisting of the LoginProvider and the key to use with that provider
        builder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

        // Limit the size of database columns
        builder.Property(ul => ul.LoginProvider).IsRequired().HasMaxLength(128);
        builder.Property(ul => ul.ProviderKey).IsRequired().HasMaxLength(128);
        builder.Property(ul => ul.Token).IsRequired(false).HasMaxLength(300);
        builder.Property(ul => ul.ProviderDisplayName).IsRequired();
        builder.Property(ul => ul.IsConfirmed).IsRequired();

        // The relationship between User and UserLogin
        builder.HasOne(ul => ul.User)
            .WithMany(u => u.Logins)
            .HasForeignKey(uc => uc.UserId)
            .IsRequired();
        
        // A concurrency token for use with the optimistic concurrency checking
        builder.UseXminAsConcurrencyToken();
    }
}