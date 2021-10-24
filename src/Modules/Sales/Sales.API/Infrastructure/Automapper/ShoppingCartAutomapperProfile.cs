using AutoMapper;

using VShop.Services.Sales.API.Models;
using VShop.Services.Sales.API.Application.Commands;
using VShop.Services.Sales.API.Application.Commands.Shared;

namespace VShop.Services.Sales.API.Infrastructure.Automapper
{
    public class ShoppingCartAutomapperProfile : Profile
    {
        public ShoppingCartAutomapperProfile()
        {
            CreateMap<CreateShoppingCartRequest, CreateShoppingCartCommand>();
            CreateMap<CreateShoppingCartProductRequest, ShoppingCartItemDto>();
            CreateMap<AddShoppingCartProductRequest, ShoppingCartItemDto>();
            CreateMap<RemoveShoppingCartProductRequest, RemoveShoppingCartProductCommand>();
            CreateMap<SetContactInformationRequest, SetContactInformationCommand>();
            CreateMap<SetDeliveryAddressRequest, SetDeliveryAddressCommand>();
        }
    }
}