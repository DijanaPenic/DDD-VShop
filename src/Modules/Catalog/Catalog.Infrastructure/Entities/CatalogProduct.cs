using System;

using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Catalog.Infrastructure.Entities
{
    public class CatalogProduct : DbEntityBase
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public CatalogCategory Category { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PictureUri { get; set; }
        public bool IsDeleted { get; set; }
        public int AvailableStock { get; set; }
        public int MaxStockThreshold { get; set; }
    }
}