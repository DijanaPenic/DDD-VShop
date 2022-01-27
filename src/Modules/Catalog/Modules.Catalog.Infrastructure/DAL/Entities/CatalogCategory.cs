using VShop.SharedKernel.PostgresDb;

namespace VShop.Modules.Catalog.Infrastructure.DAL.Entities
{
    internal class CatalogCategory : DbEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<CatalogProduct> Products { get; set; }
    }
}