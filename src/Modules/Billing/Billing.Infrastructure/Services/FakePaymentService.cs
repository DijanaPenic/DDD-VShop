using System;
using System.Threading.Tasks;

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
            DateTime cardExpiration
        )
        {
            return Task.FromResult<Result>(Result.Success);
        }
    }
}