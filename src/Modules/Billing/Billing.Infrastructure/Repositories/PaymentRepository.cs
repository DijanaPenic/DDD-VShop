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
        private readonly BillingContext _dbContext;

        public PaymentRepository(BillingContext dbContext) => _dbContext = dbContext;

        public async Task SaveAsync(Payment paymentTransfer, CancellationToken cancellationToken)
        {
            await _dbContext.Payments.AddAsync(paymentTransfer, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Payment>> GetByOrderIdAsync
        (
            Guid orderId,
            PaymentStatus status,
            CancellationToken cancellationToken
        ) => await _dbContext.Payments
            .Where(p => p.OrderId == orderId && p.Status == status)
            .ToListAsync(cancellationToken);

        public Task<bool> IsPaymentSuccessAsync(Guid orderId, PaymentType type, CancellationToken cancellationToken)
            =>  _dbContext.Payments.AnyAsync
            (
                p => p.OrderId == orderId && p.Status == PaymentStatus.Success && p.Type == type,
                cancellationToken
            ); 
    }
}