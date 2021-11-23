﻿using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace VShop.SharedKernel.PostgresDb
{
    public class DbContextBase : DbContext
    {
        private IDbContextTransaction _currentTransaction;
        
        protected DbContextBase() { }
        protected DbContextBase(DbContextOptions contextOptions) : base(contextOptions) { }
        
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => SaveChangesAsync(DateTime.UtcNow, cancellationToken);

        public Task<int> SaveChangesAsync(DateTime effectiveTime, CancellationToken cancellationToken = default)
        {
            IEnumerable<EntityEntry> entries = ChangeTracker.Entries()
                .Where(e => e.Entity is DbBaseEntity && e.State is (EntityState.Added or EntityState.Modified));

            foreach (EntityEntry entry in entries)
            {
                DbBaseEntity baseEntity = (DbBaseEntity)entry.Entity;

                baseEntity.DateUpdatedUtc = effectiveTime;
                if (entry.State == EntityState.Added) baseEntity.DateCreatedUtc = effectiveTime;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
        
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction is not null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

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
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_currentTransaction is not null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
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
                if (_currentTransaction is not null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}