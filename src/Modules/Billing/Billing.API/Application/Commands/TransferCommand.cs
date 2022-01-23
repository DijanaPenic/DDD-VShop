using System;
using NodaTime;
using NodaTime.Serialization.Protobuf;

using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.Modules.Billing.API.Application.Commands
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