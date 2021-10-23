using AutoMapper;

using VShop.Services.ShoppingCarts.API.Models;
using VShop.Services.ShoppingCarts.API.Application.Commands;
using VShop.Services.ShoppingCarts.API.Application.Commands.Shared;

namespace VShop.Services.ShoppingCarts.API.Infrastructure.Automapper
{
    public class BasketAutomapperProfile : Profile
    {
        public BasketAutomapperProfile()
        {
            CreateMap<CreateShoppingCartRequest, CreateShoppingCartCommand>();
            CreateMap<AddShoppingCartProductRequest, ShoppingCartItemDto>();
        }
    }
}