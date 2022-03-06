using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.DAL.Configurations;

internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User> 
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Maps to the User table
        builder.ToTable("user", IdentityDbContext.IdentitySchema);

        // Primary key
        builder.HasKey(u => u.Id);

        // Indexes for "normalized" username and email, to allow efficient lookups
        builder.HasIndex(u => u.NormalizedUserName).HasDatabaseName("user_name_index").IsUnique();
        builder.HasIndex(u => u.NormalizedEmail).HasDatabaseName("user_email_index");

        // Requirements
        builder.Property(u => u.UserName).IsRequired().HasMaxLength(256);
        builder.Property(u => u.NormalizedUserName).IsRequired().HasMaxLength(256);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.NormalizedEmail).IsRequired().HasMaxLength(256);
        builder.Property(u => u.DateCreated).IsRequired();
        builder.Property(u => u.DateUpdated).IsRequired();

        // The relationship between User and Role
        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<UserRole>("user_role");
        
        // A concurrency token for use with the optimistic concurrency checking
        builder.UseXminAsConcurrencyToken();
    }
}