using AutoMapper;

using VShop.Services.ShoppingCarts.API.Models;
using VShop.Services.ShoppingCarts.API.Application.Commands;
using VShop.Services.ShoppingCarts.API.Application.Commands.Shared;

namespace VShop.Services.ShoppingCarts.API.Infrastructure.Automapper
{
    public class ShoppingCartAutomapperProfile : Profile
    {
        public ShoppingCartAutomapperProfile()
        {
            CreateMap<CreateShoppingCartRequest, CreateShoppingCartCommand>();
            CreateMap<CreateShoppingCartProductRequest, ShoppingCartItemDto>();
            CreateMap<AddShoppingCartProductRequest, ShoppingCartItemDto>();
            CreateMap<RemoveShoppingCartProductRequest, RemoveShoppingCartProductCommand>();
        }
    }
}