using NodaTime.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.DAL.Configurations;

internal class ClientEntityTypeConfiguration : IEntityTypeConfiguration<Client> 
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        // Maps to the Client table
        builder.ToTable("client", IdentityDbContext.IdentitySchema);

        // Primary key
        builder.HasKey(c => c.Id);

        // Limit the size of columns
        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Description).HasMaxLength(100);
        builder.Property(c => c.Secret).IsRequired();
        builder.Property(c => c.AllowedOrigin).HasMaxLength(100);

        // Set indices
        builder.HasIndex(c => c.Name).HasDatabaseName("NameIndex").IsUnique();
        
        // TODO - change index names to snake case.
        // TODO - missing DateCreated and DateUpdated fields.
        
        // A concurrency token for use with the optimistic concurrency checking
        builder.UseXminAsConcurrencyToken();

        // Seed data
        builder.HasData
        (
            new Client
            {
                Id = Guid.Parse("5c52160a-4ab4-49c6-ba5f-56df9c5730b6"),
                Name = "WebApiApplication",
                Description = "Web API Application",
                Active = true,
                AccessTokenLifeTime = 20,
                RefreshTokenLifeTime = 60,
                Secret = "PX23zsV/7nm6+ZI9LmrKXSBf2O47cYtiJGk2WJ/G/PdU2eD7Y929MZeItkGpBY2v6a2tXhGINq8bAQYz1bQC6A==",
                AllowedOrigin = "*",
                DateCreated = InstantPattern.General.Parse("2022-01-01T00:00:00Z").Value,
                DateUpdated = InstantPattern.General.Parse("2022-01-01T00:00:00Z").Value
            }
        );
    }
}