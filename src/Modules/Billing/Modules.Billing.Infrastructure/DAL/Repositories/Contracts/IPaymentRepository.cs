using VShop.Modules.Billing.Infrastructure.DAL.Entities;

namespace VShop.Modules.Billing.Infrastructure.DAL.Repositories.Contracts
{
    internal interface IPaymentRepository
    {
        Task SaveAsync(Transfer transfer, CancellationToken cancellationToken);
        Task<Transfer> GetPaidPaymentAsync(Guid orderId, CancellationToken cancellationToken);
    }
}