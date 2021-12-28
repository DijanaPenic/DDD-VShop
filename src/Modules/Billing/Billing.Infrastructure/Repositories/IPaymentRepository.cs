using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.Modules.Billing.Infrastructure.Entities;

namespace VShop.Modules.Billing.Infrastructure.Repositories
{
    public interface IPaymentRepository
    {
        Task SaveAsync(Payment paymentTransfer, CancellationToken cancellationToken);
        Task<IReadOnlyList<Payment>> GetByOrderIdAsync
        (
            Guid orderId,
            PaymentStatus status,
            CancellationToken cancellationToken
        );

        Task<bool> IsPaymentSuccessAsync(Guid orderId, PaymentType type, CancellationToken cancellationToken);
    }
}