using AutoMapper;
using Google.Type;

using VShop.Modules.Sales.API.Models;
using VShop.Modules.Sales.Infrastructure.Commands;
using VShop.Modules.Sales.Infrastructure.Commands.Shared;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.Modules.Sales.API.Automapper
{
    internal class ShoppingCartAutomapperProfile : Profile
    {
        public ShoppingCartAutomapperProfile()
        {
            CreateMap<CreateShoppingCartRequest, CreateShoppingCartCommand>();
            CreateMap<CreateShoppingCartRequest.ShoppingCartProductRequestDto, ShoppingCartProductCommandDto>();
            CreateMap<AddShoppingCartProductRequest, ShoppingCartProductCommandDto>();
            CreateMap<RemoveShoppingCartProductRequest, RemoveShoppingCartProductCommand>();
            CreateMap<SetContactInformationRequest, SetContactInformationCommand>();
            CreateMap<SetDeliveryAddressRequest, SetDeliveryAddressCommand>();
            CreateMap<decimal, Money>().ConvertUsing(src => src.ToMoney());
        }
    }
}