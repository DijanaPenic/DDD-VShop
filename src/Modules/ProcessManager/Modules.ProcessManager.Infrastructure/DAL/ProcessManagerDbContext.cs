using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.PostgresDb.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

[assembly: InternalsVisibleTo("Database.DatabaseMigrator")]
namespace VShop.Modules.ProcessManager.Infrastructure.DAL
{
    internal class ProcessManagerDbContext : DbContextBase
    {
        public ProcessManagerDbContext(IClockService clockService, IDbContextBuilder contextBuilder) 
            : base(clockService, contextBuilder) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => ContextBuilder.ConfigureContext(optionsBuilder);
    
        protected override void OnModelCreating(ModelBuilder builder)
            => builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}