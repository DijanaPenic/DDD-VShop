using System;
using System.Collections.Generic;

using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Catalog.Infrastructure.Entities
{
    public class CatalogCategory : DbEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<CatalogProduct> Products { get; set; }
    }
}