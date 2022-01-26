using VShop.SharedKernel.PostgresDb;

namespace VShop.SharedKernel.Subscriptions.DAL.Entities
{
    public class Checkpoint : DbEntity
    {
        public string SubscriptionId { get; set; }
        public ulong? Position { get; set; }
    }
}