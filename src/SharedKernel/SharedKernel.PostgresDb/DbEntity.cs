using NodaTime;

namespace VShop.SharedKernel.PostgresDb
{
    public abstract class DbEntity
    {
        public Instant DateCreated { get; set; }
        public Instant DateUpdated { get; set; }
    }
}