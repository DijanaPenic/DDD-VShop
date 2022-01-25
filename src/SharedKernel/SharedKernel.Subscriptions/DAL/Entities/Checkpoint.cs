using VShop.SharedKernel.PostgresDb;

namespace VShop.SharedKernel.Subscriptions.DAL.Entities
{
    public class Checkpoint : DbEntityBase
    {
        public string SubscriptionId { get; set; }
        public ulong? Position { get; set; }
    }
}