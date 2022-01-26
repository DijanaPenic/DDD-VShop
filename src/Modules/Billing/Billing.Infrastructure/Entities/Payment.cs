using System;

using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Billing.Infrastructure.Entities
{
    public class Payment : DbEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public PaymentType Type { get; set; }
        public PaymentStatus Status { get; set; }
        public string Error { get; set; }
    }
}