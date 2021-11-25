using System;

namespace VShop.SharedKernel.PostgresDb
{
    public class DbEntityBase
    {
        public DateTime DateCreatedUtc { get; set; }
        public DateTime DateUpdatedUtc { get; set; }
    }
}