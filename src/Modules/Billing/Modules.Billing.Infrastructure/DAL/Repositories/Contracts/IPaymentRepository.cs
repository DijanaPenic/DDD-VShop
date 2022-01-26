using VShop.Modules.Billing.Infrastructure.DAL.Entities;

namespace VShop.Modules.Billing.Infrastructure.DAL.Repositories.Contracts
{
    internal interface IPaymentRepository
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