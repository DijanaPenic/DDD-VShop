using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.PostgresDb.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.Modules.Sales.Infrastructure.DAL.Entities;

[assembly: InternalsVisibleTo("Database.DatabaseMigrator")]
namespace VShop.Modules.Sales.Infrastructure.DAL
{
    internal class SalesDbContext : DbContextBase
    {
        public const string ShoppingCartSchema = "shopping_cart";
        public const string OrderSchema = "order";

        public DbSet<ShoppingCartInfo> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartInfoItem> ShoppingCartItems { get; set; }

        public SalesDbContext(IClockService clockService, IDbContextBuilder contextBuilder) 
            : base(clockService, contextBuilder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => ContextBuilder.ConfigureContext(optionsBuilder);
    
        protected override void OnModelCreating(ModelBuilder builder)
            => builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}