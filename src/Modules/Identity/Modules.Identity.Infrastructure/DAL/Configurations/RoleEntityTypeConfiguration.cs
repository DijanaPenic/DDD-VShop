using NodaTime.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.DAL.Configurations;

internal class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role> 
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Maps to the Role table
            builder.ToTable("role", IdentityDbContext.IdentitySchema);

            // Primary key
            builder.HasKey(r => r.Id);

            // Index for "normalized" role name to allow efficient lookups
            builder.HasIndex(r => r.NormalizedName).HasDatabaseName("role_name_index").IsUnique();

            // Requirements
            builder.Property(r => r.Name).IsRequired().HasMaxLength(256);
            builder.Property(r => r.NormalizedName).IsRequired().HasMaxLength(256);
            builder.Property(r => r.DateCreated).IsRequired();
            builder.Property(r => r.DateUpdated).IsRequired();

            // A concurrency token for use with the optimistic concurrency checking
            builder.UseXminAsConcurrencyToken();

            // Seed data
            builder.HasData
            (
                new Role
                {
                    Id = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Stackable = false,
                    DateCreated = InstantPattern.General.Parse("2022-01-01T00:00:00Z").Value,
                    DateUpdated = InstantPattern.General.Parse("2022-01-01T00:00:00Z").Value
                },
                new Role
                {
                    Id = Guid.Parse("d82ef5e5-f08a-4173-b83a-74618893891b"),
                    Name = "Customer",
                    NormalizedName = "CUSTOMER",
                    Stackable = true,
                    DateCreated = InstantPattern.General.Parse("2022-01-01T00:00:00Z").Value,
                    DateUpdated = InstantPattern.General.Parse("2022-01-01T00:00:00Z").Value
                },
                new Role
                {
                    Id = Guid.Parse("d92ef5e5-f08a-4173-b83a-74618893891b"),
                    Name = "Store Manager",
                    NormalizedName = "STORE MANAGER",
                    Stackable = true,
                    DateCreated = InstantPattern.General.Parse("2022-01-01T00:00:00Z").Value,
                    DateUpdated = InstantPattern.General.Parse("2022-01-01T00:00:00Z").Value
                },
                new Role
                {
                    Id = Guid.Parse("9621c09c-06b1-45fb-8baf-38e0757e2f59"),
                    Name = "Guest",
                    NormalizedName = "GUEST",
                    Stackable = false,
                    DateCreated = InstantPattern.General.Parse("2022-01-01T00:00:00Z").Value,
                    DateUpdated = InstantPattern.General.Parse("2022-01-01T00:00:00Z").Value
                }
            );
        }
    }