using NodaTime;
using NodaTime.Serialization.Protobuf;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Billing.Infrastructure.Commands
{
    public partial class TransferCommand : MessageContext, ICommand
    {
        public TransferCommand
        (
            Guid orderId,
            decimal amount,
            int cardTypeId,
            string cardNumber,
            string cardSecurityNumber,
            string cardholderName,
            Instant cardExpiration
        )
        {
            OrderId = orderId;
            Amount = amount.ToMoney();
            CardTypeId = cardTypeId;
            CardNumber = cardNumber;
            CardSecurityNumber = cardSecurityNumber;
            CardholderName = cardholderName;
            CardExpiration = cardExpiration.ToTimestamp();
        }
    }
}