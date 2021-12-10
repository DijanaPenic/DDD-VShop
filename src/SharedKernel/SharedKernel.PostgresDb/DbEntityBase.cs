using NodaTime;

namespace VShop.SharedKernel.PostgresDb
{
    public class DbEntityBase
    {
        public Instant DateCreated { get; set; }
        public Instant DateUpdated { get; set; }
    }
}