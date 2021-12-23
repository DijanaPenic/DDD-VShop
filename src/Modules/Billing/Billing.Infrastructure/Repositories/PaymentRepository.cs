using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Billing.Infrastructure.Entities;

namespace VShop.Modules.Billing.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly BillingContext _dbContext;

        public PaymentRepository(BillingContext dbContext) => _dbContext = dbContext;

        public async Task SaveAsync(PaymentTransfer paymentTransfer, CancellationToken cancellationToken)
        {
            await _dbContext.Payments.AddAsync(paymentTransfer, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}