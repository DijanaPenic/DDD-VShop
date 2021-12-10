﻿using System;
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
            int cardTypeId,
            string cardNumber,
            string cardSecurityNumber,
            string cardholderName,
            Instant cardExpiration,
            CancellationToken cancellationToken = default
        );
    }
}