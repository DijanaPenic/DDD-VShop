using System;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;

using VShop.SharedKernel.Infrastructure;

namespace VShop.Modules.Billing.Infrastructure.Services
{
    public class FakePaymentService : IPaymentService
    {
        public FakePaymentService()
        {
            
        }

        public Task<Result> TransferAsync
        (
            Guid orderId,
            int cardTypeId,
            string cardNumber,
            string cardSecurityNumber,
            string cardholderName,
            Instant cardExpiration,
            CancellationToken cancellationToken = default
        )
        {
            return Task.FromResult<Result>(Result.Success);
        }
    }
}