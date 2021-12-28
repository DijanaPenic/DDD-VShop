using System;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;

using VShop.SharedKernel.Infrastructure;

namespace VShop.Modules.Billing.Infrastructure.Services
{
    public interface IPaymentService
    {
        Task<Result> TransferAsync
        (
            Guid orderId,
            decimal amount,
            int cardTypeId,
            string cardNumber,
            string cardSecurityNumber,
            string cardholderName,
            Instant cardExpiration,
            CancellationToken cancellationToken = default
        );
        
        Task<Result> RefundAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default);
    }
}