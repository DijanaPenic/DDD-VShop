using AutoMapper;

using VShop.Modules.Catalog.API.Models;
using VShop.Modules.Catalog.Infrastructure.Entities;

namespace VShop.Modules.Catalog.API.Infrastructure.Automapper
{
    public class CatalogAutomapperProfile : Profile
    {
        public CatalogAutomapperProfile()
        {
            CreateMap<ProductRequest, CatalogProduct>();
            CreateMap<CategoryRequest, CatalogCategory>();
        }
    }
}