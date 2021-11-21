using System;

using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Billing.Infrastructure.Entities
{
    public class PaymentTransfer : DbBaseEntity
    {
        public Guid OrderId { get; set; }
        public PaymentTransferStatus Status { get; set; }
    }
}