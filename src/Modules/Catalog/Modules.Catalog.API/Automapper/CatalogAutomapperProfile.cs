using AutoMapper;

using VShop.Modules.Catalog.API.Models;
using VShop.Modules.Catalog.Infrastructure.DAL.Entities;

namespace VShop.Modules.Catalog.API.Automapper
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