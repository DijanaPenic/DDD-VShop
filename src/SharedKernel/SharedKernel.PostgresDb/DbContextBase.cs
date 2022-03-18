using NodaTime;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

using VShop.SharedKernel.PostgresDb.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.SharedKernel.PostgresDb
{
    public abstract class DbContextBase : DbContext
    {
        private readonly IClockService _clockService;
        protected readonly IDbContextBuilder ContextBuilder;
        public IDbContextTransaction CurrentTransaction { get; private set; }

        protected DbContextBase() { }

        public DbContextBase(IClockService clockService, IDbContextBuilder contextBuilder)
        {
            _clockService = clockService;
            ContextBuilder = contextBuilder;    
        }
        
        protected DbContextBase(DbContextOptions dbContextOptions) : base(dbContextOptions) { }
        
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => SaveChangesAsync(_clockService.Now, cancellationToken);

        public Task<int> SaveChangesAsync(Instant now, CancellationToken cancellationToken = default)
        {
            IEnumerable<EntityEntry> entries = ChangeTracker.Entries()
                .Where(e => e.Entity is DbEntity && e.State is (EntityState.Added or EntityState.Modified));

            foreach (EntityEntry entry in entries)
            {
                DbEntity baseEntity = (DbEntity)entry.Entity;

                baseEntity.DateUpdated = now;
                if (entry.State == EntityState.Added) baseEntity.DateCreated = now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
        
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (CurrentTransaction is not null) return null;

            CurrentTransaction = await Database.BeginTransactionAsync(cancellationToken);

            return CurrentTransaction;
        }
        
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (CurrentTransaction is null) return;
            
            try
            {
                await SaveChangesAsync(cancellationToken);
                await CurrentTransaction.CommitAsync(cancellationToken);
                
                await DisposeTransactionAsync();
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
        
        private async Task DisposeTransactionAsync()
        {
            if (CurrentTransaction is null) return;
            
            await CurrentTransaction.DisposeAsync();
            CurrentTransaction = null;
        }
        
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (CurrentTransaction is null) return;
            
            try
            {
                await CurrentTransaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().Add(entity);
            return Task.CompletedTask;
        }
        
        public async Task DeleteByKeyAsync<TEntity>(CancellationToken cancellationToken, params object[] keyValues)
            where TEntity : class
        {
            TEntity entity = await Set<TEntity>().FindAsync(keyValues, cancellationToken);
            if (entity is not null) Set<TEntity>().Remove(entity);
        }
        
        public Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().Remove(entity);
            return Task.CompletedTask;
        }
        
        public Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public ValueTask<TEntity> FindByKeyAsync<TEntity>(CancellationToken cancellationToken, params object[] keyValues) 
            where TEntity : class
            => Set<TEntity>().FindAsync(keyValues, cancellationToken);
    }
}