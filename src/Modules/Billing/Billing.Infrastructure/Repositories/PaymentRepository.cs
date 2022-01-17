using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Billing.Infrastructure.Entities;

namespace VShop.Modules.Billing.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly BillingDbContext _dbDbContext;

        public PaymentRepository(BillingDbContext dbDbContext) => _dbDbContext = dbDbContext;

        public async Task SaveAsync(Payment paymentTransfer, CancellationToken cancellationToken)
        {
            await _dbDbContext.Payments.AddAsync(paymentTransfer, cancellationToken);
            await _dbDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Payment>> GetByOrderIdAsync
        (
            Guid orderId,
            PaymentStatus status,
            CancellationToken cancellationToken
        ) => await _dbDbContext.Payments
            .Where(p => p.OrderId == orderId && p.Status == status)
            .ToListAsync(cancellationToken);

        public Task<bool> IsPaymentSuccessAsync(Guid orderId, PaymentType type, CancellationToken cancellationToken)
            =>  _dbDbContext.Payments.AnyAsync
            (
                p => p.OrderId == orderId && p.Status == PaymentStatus.Success && p.Type == type,
                cancellationToken
            ); 
    }
}