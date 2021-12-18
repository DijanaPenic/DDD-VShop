using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using NodaTime;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.SharedKernel.PostgresDb
{
    public class DbContextBase : DbContext
    {
        private IDbContextTransaction _currentTransaction;
        private readonly IClockService _clockService;
        
        protected readonly IDbContextBuilder ContextBuilder;
        
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
                .Where(e => e.Entity is DbEntityBase && e.State is (EntityState.Added or EntityState.Modified));

            foreach (EntityEntry entry in entries)
            {
                DbEntityBase baseEntity = (DbEntityBase)entry.Entity;

                baseEntity.DateUpdated = now;
                if (entry.State == EntityState.Added) baseEntity.DateCreated = now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
        
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is not null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);

            return _currentTransaction;
        }
        
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;
        public bool HasActiveTransaction => _currentTransaction is not null;

        public async Task CommitTransactionAsync
        (
            IDbContextTransaction transaction,
            CancellationToken cancellationToken = default
        )
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                
                DisposeCurrentTransaction();
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
        
        private Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return _currentTransaction?.RollbackAsync(cancellationToken);
            }
            finally
            {
                DisposeCurrentTransaction();
            }
        }

        private void DisposeCurrentTransaction()
        {
            if (_currentTransaction is null) return;
            
            _currentTransaction.Dispose();
            _currentTransaction = null;
        }
    }
}