using System;

using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Billing.Infrastructure.Entities
{
    public class PaymentTransfer : DbEntityBase
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public PaymentTransferStatus Status { get; set; }
        public string Error { get; set; }
    }
}