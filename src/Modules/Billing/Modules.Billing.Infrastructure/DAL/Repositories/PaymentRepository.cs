using Microsoft.EntityFrameworkCore;

using VShop.Modules.Billing.Infrastructure.DAL.Entities;
using VShop.Modules.Billing.Infrastructure.DAL.Repositories.Contracts;

namespace VShop.Modules.Billing.Infrastructure.DAL.Repositories
{
    internal class PaymentRepository : IPaymentRepository
    {
        private readonly BillingDbContext _dbDbContext;

        public PaymentRepository(BillingDbContext dbDbContext) => _dbDbContext = dbDbContext;

        public async Task SaveAsync(Transfer transfer, CancellationToken cancellationToken)
        {
            await _dbDbContext.Transfers.AddAsync(transfer, cancellationToken);
            await _dbDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Transfer> GetPaidPaymentAsync
        (
            Guid orderId,
            CancellationToken cancellationToken
        ) => await _dbDbContext.Transfers.SingleOrDefaultAsync(p => 
                    p.OrderId == orderId &&
                    p.Status == TransferStatus.Success &&
                    p.Type == TransferType.Payment,
                cancellationToken);
    }
}