using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.Modules.Billing.Infrastructure.Entities;

namespace VShop.Modules.Billing.Infrastructure.Repositories
{
    public interface IPaymentRepository
    {
        Task SaveAsync(PaymentTransfer paymentTransfer, CancellationToken cancellationToken);
        Task<IReadOnlyList<PaymentTransfer>> GetByOrderIdAsync
        (
            Guid orderId,
            PaymentTransferStatus status,
            CancellationToken cancellationToken
        );

        Task<bool> IsOrderPaidAsync(Guid orderId, CancellationToken cancellationToken);
    }
}