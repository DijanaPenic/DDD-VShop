using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.PostgresDb.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.Modules.Catalog.Infrastructure.DAL.Entities;

[assembly: InternalsVisibleTo("Database.DatabaseMigrator")]
namespace VShop.Modules.Catalog.Infrastructure.DAL
{
    internal class CatalogDbContext : DbContextBase
    {
        public const string CatalogSchema = "catalog";

        public DbSet<CatalogProduct> Products { get; set; }
        public DbSet<CatalogCategory> Categories { get; set; }

        public CatalogDbContext(IClockService clockService, IDbContextBuilder contextBuilder) 
            : base(clockService, contextBuilder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => ContextBuilder.ConfigureContext(optionsBuilder);

        protected override void OnModelCreating(ModelBuilder builder)
            => builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}