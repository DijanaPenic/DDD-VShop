using System;

namespace VShop.SharedKernel.PostgresDb
{
    public class DbBaseEntity
    {
        public DateTime DateCreatedUtc { get; set; }
        public DateTime DateUpdatedUtc { get; set; }
    }
}