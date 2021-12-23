using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Billing.Infrastructure.Entities;

namespace VShop.Modules.Billing.Infrastructure.Repositories
{
    public interface IPaymentRepository
    {
        Task SaveAsync(PaymentTransfer paymentTransfer, CancellationToken cancellationToken);
    }
}