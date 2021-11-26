using VShop.SharedKernel.PostgresDb;

namespace VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure.Entities
{
    public class Checkpoint : DbEntityBase
    {
        public string SubscriptionId { get; set; }
        public ulong? Position { get; set; }
    }
}