using NodaTime;

using VShop.SharedKernel.Infrastructure;
using VShop.Modules.Billing.Infrastructure.Services.Contracts;

namespace VShop.Modules.Billing.Infrastructure.Services
{
    public class FakePaymentService : IPaymentService
    {
        public FakePaymentService()
        {
            
        }

        public Task<Result> TransferAsync
        (
            Guid orderId, decimal amount, int cardTypeId, string cardNumber, string cardSecurityNumber,
            string cardholderName, Instant cardExpiration, CancellationToken cancellationToken = default
        ) => Task.FromResult<Result>(Result.Success);
        
        public Task<Result> RefundAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default)
            => Task.FromResult<Result>(Result.Success);
    }
}