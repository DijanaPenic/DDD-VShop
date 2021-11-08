using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace VShop.SharedKernel.PostgresDb
{
    public class ApplicationDbContextBase : DbContext
    {
        protected ApplicationDbContextBase(DbContextOptions contextOptions)
            : base(contextOptions)
        {
        }
        
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => SaveChangesAsync(DateTime.UtcNow, cancellationToken);

        public Task<int> SaveChangesAsync(DateTime effectiveTime, CancellationToken cancellationToken = default)
        {
            IEnumerable<EntityEntry> entries = ChangeTracker.Entries()
                .Where(e => e.Entity is DbBaseEntity && e.State is (EntityState.Added or EntityState.Modified));

            foreach (EntityEntry entry in entries)
            {
                DbBaseEntity baseEntity = entry.Entity as DbBaseEntity;

                baseEntity.DateUpdatedUtc = effectiveTime;
                if (entry.State == EntityState.Added) baseEntity.DateCreatedUtc = effectiveTime;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}