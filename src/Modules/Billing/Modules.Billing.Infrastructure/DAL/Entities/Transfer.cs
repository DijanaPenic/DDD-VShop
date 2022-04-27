using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Billing.Infrastructure.DAL.Entities
{
    internal class Transfer : DbEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public long Amount { get; set; }
        public TransferType Type { get; set; }
        public TransferStatus Status { get; set; }
        public string Error { get; set; }
        public string IntentId { get; set; }
    }
}