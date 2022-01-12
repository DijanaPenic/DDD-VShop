using System;
using NodaTime;
using NodaTime.Serialization.Protobuf;

using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Billing.API.Application.Commands
{
    public partial class TransferCommand : IBaseCommand
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
            Amount = amount;
            CardTypeId = cardTypeId;
            CardNumber = cardNumber;
            CardSecurityNumber = cardSecurityNumber;
            CardholderName = cardholderName;
            CardExpiration = cardExpiration.ToTimestamp();
        }
    }
}